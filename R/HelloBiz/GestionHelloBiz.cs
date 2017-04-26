using System;
using System.Collections.Generic;
using System.Linq;
using R.Web;

namespace R
{
    public class GestionHelloBiz
    {
        public event Action<List<ArticleHelloBiz>> NewArticle;
        public event Action<string> Erreur;
        public RFichier GestionFichier;
        private List<ArticleHelloBiz> _listeArticle;
        private readonly string _nameXmlTour;

        public GestionHelloBiz()
        {
            try
            {
                _nameXmlTour = RFichier.DirectoryAppWithFile("hellobiz.xml");
                GestionFichier = new RFichier(_nameXmlTour);
                if (GestionFichier.VerifFichier.IsFalse())
                {
                    OnErreur("Le fichier des concerts est introuvable.");
                    return;
                }
                _listeArticle = _nameXmlTour.FileXmlUnserialize<List<ArticleHelloBiz>>();
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        public bool GetNewArticle(string search = null, int time = 20)
        {
            try
            {
                if (search.IsNullOrEmpty())
                {
                    OnErreur("Le lien de recherche est vide ou nul.");
                    return false;
                }

                RThread.DoAsync(() =>
                {
                    bool firstPassage = true;
                    while (true)
                    {
                        if(firstPassage.IsFalse())
                            time.SleepToMin();
                        firstPassage = false;

                        var document = search.RloadUrl();
                        if (document.IsNull())
                        {
                            OnErreur("Erreur lors du chargement de la page Hello Biz.");
                            return;
                        }

                        //On select les actu
                        var nodeActu = document.DocumentNode.SelectNodes("//*[@class='row listing grid-overlay']//div[@class='column half']");
                        if (nodeActu.IsEmpty())
                        {
                            OnErreur("Le lien xpath d'hello biz n'est plus valide.");
                            return;
                        }

                        //On crée une liste temporaire des nouveaux articles hellobiz
                        var listeTemp = new List<ArticleHelloBiz>();
                        foreach (var node in nodeActu)
                        {
                            if (node.IsNull()) continue;
                            //On get le titre
                            var nodeImage = node.SelectSingleNode(".//img");
                            if (nodeImage.IsNull()) continue;
                            var attrTitle = nodeImage.Attributes["title"];
                            if (attrTitle.IsNull()) continue;

                            //On regarde si on a déjà un article avec le même titre
                            if (_listeArticle.IsNull())
                            {
                                listeTemp.Add(new ArticleHelloBiz { Titre = attrTitle.Value });
                                continue;
                            }

                            ArticleHelloBiz art = _listeArticle.FirstOrDefault(articleHelloBiz => articleHelloBiz.Titre.Equals(attrTitle.Value));
                            if (art.IsNotNull()) continue;
                            listeTemp.Add(new ArticleHelloBiz { Titre = attrTitle.Value });
                        }

                        if (listeTemp.IsEmpty()) continue;

                        //On ajoute les nouveaux articles au début de la liste
                        _listeArticle = _listeArticle.Concat(listeTemp, false);
                        if (_listeArticle.IsNull()) continue;

                        //On modifie le fichier xml
                        if (_listeArticle.XmlSerialize(_nameXmlTour).IsFalse())
                        {
                            OnErreur("Erreur lors de la sauvegarde des articles d'hello bizs.");
                            continue;
                        }

                        //On déclenche l'event
                        OnNewArticle(listeTemp);
                    }
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private void OnNewArticle(List<ArticleHelloBiz> obj)
        {
            var handler = NewArticle;
            if (handler != null) handler(obj);
        }

        private void OnErreur(string obj)
        {
            var handler = Erreur;
            if (handler != null) handler(obj);
        }
    }

    public class ArticleHelloBiz
    {
        public string Titre { get; set; }
    }
}
