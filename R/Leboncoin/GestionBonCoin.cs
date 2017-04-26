using System;
using System.Collections.Generic;
using R.Web;

// ReSharper disable once CheckNamespace
namespace R
{
    public class GestionBonCoin
    {
        public event Action<List<ArticleLeBonCoin>> NewArticle;
        public event Action<string> ErreurRecherche;
        public RFichier GestionFichier;
        private string _hrefLastDemande;

        public GestionBonCoin()
        {
            GestionFichier = new RFichier(RFichier.DirectoryAppWithFile("leboncoin.txt"));
            _hrefLastDemande = GestionFichier.LireLeFichier();
        }

        public bool SearchNewDemande(string lienSearch = null, int timerSearch = 10)
        {
            try
            {
                if (lienSearch.IsNullOrEmpty()) return false;
                if (_hrefLastDemande.IsNotNullOrEmpty())
                    _hrefLastDemande = _hrefLastDemande.Replace("\n", string.Empty);
                

                //On lance le thread de recherche
                RThread.DoAsync(() =>
                {
                    var firstPassage = true;
                    var globalListeArticle = new List<ArticleLeBonCoin>();
                    while(true)
                    {
                        if(!firstPassage)
                            timerSearch.SleepToMin();
                        firstPassage = false;
                        //On charge la page
                        var document = lienSearch.RloadUrl();
                        if (document.IsNull())
                        {
                            break;
                        }

                        //On recup les liens des articles
                        var listeArticle = document.DocumentNode.SelectNodes("//*[@class='list-lbc']//a");
                        if (listeArticle.IsEmpty()) continue;

                        foreach (var article in listeArticle)
                        {
                            //On regarde si il a le même href que le dernier qu'on a save
                            var hrefnode = article.Attributes["href"];
                            if (hrefnode.IsNull()) continue;
                            var href = hrefnode.Value;
                            if (href.IsNullOrEmpty()) continue;
                            if (href.Equals(_hrefLastDemande)) break;

                            //On recup le titre
                            var titrenode = article.SelectSingleNode(".//*[@class='title']");
                            if (titrenode.IsNull()) continue;
                            var title = titrenode.InnerText;
                            if (title.IsNullOrEmpty()) continue;

                            globalListeArticle.Add(new ArticleLeBonCoin
                            {
                                Href = href,
                                Titre = title
                            });
                        }

                        //On save le dernier lien trouvé
                        if (globalListeArticle.IsEmpty()) continue;
                        _hrefLastDemande = globalListeArticle.GetFirstElement().Href;
                        GestionFichier.EcritureFichier(new[]
                        {
                            _hrefLastDemande
                        });

                        //On déclenche l'event
                        OnNewArticle(globalListeArticle);

                        //On vide la liste
                        globalListeArticle.Clear();
                    }
                });

                return true;
            }
            catch (Exception)
            {
                OnErreurRecherche("La recherche sur le bon coin est interrompue.");
                return false;
            }   
        }

        protected virtual void OnNewArticle(List<ArticleLeBonCoin> obj)
        {
            var handler = NewArticle;
            if (handler != null) handler(obj);
        }

        protected virtual void OnErreurRecherche(string obj)
        {
            var handler = ErreurRecherche;
            if (handler != null) handler(obj);
        }
    }

    public class ArticleLeBonCoin
    {
        public string Titre { get; set; }
        public string Href { get; set; }
    }
}
