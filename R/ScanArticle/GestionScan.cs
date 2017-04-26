using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using R.ScanArticle;
using R.Web;
using HtmlAgilityPack;

// ReSharper disable once CheckNamespace

namespace R
{
    public class GestionScan
    {
        private static Mutex mutex_event;
        public GestionScan()
        {
            //Event
            RequeteHttp.RLoadPageCompleted += LoadPageCompleted;
            RequeteHttp.ErrorLoadingPage += ErrorLoadingPage;
            mutex_event = new Mutex();
        }

        public static event Action<Article> NewArticle;
        public static event Action<int> ErrorGetElementPage;

        private static void OnErrorGetElementPage(int IdSite)
        {
            ErrorGetElementPage?.Invoke(IdSite);
        }

        private static void OnNewArticle(Article obj)
        {

            NewArticle?.Invoke(obj);
            3.SleepToSec();
        }

        /// <summary>
        ///     Création d'un thread pour écouter les news d'une page web
        /// </summary>
        /// <param name="balise">L'objet xpath pour les Xpath</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool RunScan(List<XpathArticle> balise = null, Action error = null)
        {
            try
            {
                if (balise.IsNull() || balise.IsEmpty()) return false;
                //1.SleepToMin();
                RThread.DeclareAsync(() =>
                {
                    while (true)
                    {
                        foreach (
                            // ReSharper disable once UnusedVariable
                            var baliseArticle in
                                balise.Where(baliseArticle => !baliseArticle.UrlBase.RloadUrl(baliseArticle))
                                    .Where(baliseArticle => error.IsNotNull()))
                        {
                            error?.Invoke();
                        }
                    }
                    // ReSharper disable once FunctionNeverReturns
                }).Start();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void RunScanOneArticleAsynch(List<XpathArticle> balise = null)
        {
            try
            {
                RThread.DoAsync(() => RunScanOneArticleSynch(balise));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void RunScanOneArticleAsynch(int nbArticlePerThread = 1, List<XpathArticle> balise = null, int timer = 0)
        {
            try
            {
                if (balise.IsEmpty()) return;

                var index = 0;

                while (index < balise.Count)
                {
                    var sub_liste = balise.GetRange(index, nbArticlePerThread);
                    RThread.DoAsync(() => RunScanOneArticleSynch(sub_liste, timer));
                    index += nbArticlePerThread;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void RunScanOneArticleSynch(List<XpathArticle> balise = null, int timer = 0)
        {
            try
            {
                if (balise.IsNull() || balise.IsEmpty()) return;

                var firstPassage = true;
                //1.SleepToMin();
                while (true)
                {
                    if (firstPassage.IsFalse())
                    {
                        timer.SleepToMin();
                    }

                    foreach (
                        var baliseArticle in
                            // ReSharper disable once PossibleNullReferenceException
                            balise)
                    {
                        var document = baliseArticle.UrlBase.RloadUrl();
                        if (document.IsNull())
                            continue;
                        string image = null;
                        string url = null;

                        var g =
                            baliseArticle.Params.Where(param => !param.IsNull())
                                .Where(param => param.Key.Equals("IdSite"));

                        var idSite = g.First().Value.ToInt();

                        if (g.First().Value.ToInt() == 13)
                        {
                            var t = "ok";
                        }

                        //le lien
                        foreach (var s2 in baliseArticle.Href)
                        {
                            if (s2.Chemin.IsNullOrEmpty()) continue;
                            HtmlNode nodeUrl = document.DocumentNode.SelectSingleNode(s2.Chemin);
                            if (!nodeUrl.IsNull())
                            {
                                if (s2.Attribute.IsNullOrEmpty()) continue;
                                var attr = nodeUrl.Attributes[s2.Attribute];
                                if (!attr.IsNull())
                                {
                                    url = attr.Value;
                                    url = url.AddHost(baliseArticle.UrlBase);
                                    break;
                                }
                            }
                        }


                        if (url.IsNullOrEmpty())
                        {
                            OnErrorGetElementPage(idSite);
                            continue;
                        }
                        

                        ////on regarde si on a pas déjà eu l'article
                        //bool b = false;
                        //foreach (string s in oldUrl.Where(s => !s.IsNullOrEmpty()).Where(s => s.Equals(url)))
                        //{
                        //    b = true;
                        //}

                        //if (b) continue;
                        //oldUrl.Add(url);

                        //titre
                        string titre = null;
                        foreach (var s2 in baliseArticle.Titre)
                        {
                            if (s2.Chemin.IsNullOrEmpty()) continue;
                            var nodeTitre = document.DocumentNode.SelectSingleNode(s2.Chemin);
                            if (!nodeTitre.IsNull())
                            {
                                if (s2.Attribute.IsNotNullOrEmpty())
                                {
                                    var attr = nodeTitre.Attributes[s2.Attribute];
                                    if (attr.IsNotNull())
                                    {
                                        titre = attr.Value;
                                        if(titre.IsNotNullOrEmpty())
                                            break;
                                    }
                                }
                                titre = nodeTitre.InnerText;
                                break;
                            }
                        }
                        if (titre.IsNullOrEmpty())
                        {
                            OnErrorGetElementPage(idSite);
                            continue;
                        }

                        //image
                        foreach (var s2 in baliseArticle.Image)
                        {
                            if (s2.Chemin.IsNullOrEmpty()) continue;
                            HtmlNode nodeImage = document.DocumentNode.SelectSingleNode(s2.Chemin);
                            if (!nodeImage.IsNull())
                            {
                                if (s2.Attribute.IsNullOrEmpty()) continue;
                                var attrSrc = nodeImage.Attributes[s2.Attribute];
                                if (!attrSrc.IsNull() && attrSrc.Value.IsNotNullOrEmpty())
                                {
                                    //On supprime les eventuels param après l'url de l'image
                                    attrSrc.Value = attrSrc.Value.RemoveParameter('?');
                                    var extension = attrSrc.Value.GetExtension();
                                    if (extension.IsNullOrEmpty()) continue;
                                    if (extension.IsExtensionImage().IsFalse()) continue;
                                    image = attrSrc.Value;
                                    //Traitement sur le lien de l'image
                                    image = image.AddHost(baliseArticle.UrlBase);
                                    image = image.Replace("amp;", string.Empty);
                                    break;
                                }
                            }
                        }

                        //contenu
                        string contenu = null;
                        foreach (var s2 in baliseArticle.ContenuArticle)
                        {
                            if (s2.Chemin.IsNullOrEmpty()) continue;
                            var nodeContenu = document.DocumentNode.SelectSingleNode(s2.Chemin);
                            if (!nodeContenu.IsNull())
                            {
                                contenu = nodeContenu.InnerText;
                                if (contenu.IsNotNullOrEmpty())
                                    break;
                            }
                        }

                        //Check
                        if (url.IsNullOrEmpty() || titre.IsNullOrEmpty())
                        {
                            continue;
                        }

                        var article = new Article
                        {
                            Url = url,
                            Titre = titre,
                            Params = baliseArticle.Params,
                            ContenuArticle = contenu,
                            Image = image
                        };
                        OnNewArticle(article);
                    }

                    firstPassage = false;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        ///     Méthode qui est déclenchée lorsqu'il y a une erreur de chargement d'une page web
        /// </summary>
        /// <param name="obj">L'erreur</param>
        private static void ErrorLoadingPage(string obj)
        {
            MessageBox.Show(obj);
        }

        /// <summary>
        ///     Event déclenché lorsque une page web est chargée
        /// </summary>
        /// <param name="page">La page web</param>
        /// <param name="url">L'url de la page</param>
        /// <param name="xpath">L'objet pour les balises Xpath</param>
        private void LoadPageCompleted(HtmlDocument page, string url, XpathArticle xpath)
        {
            try
            {
                //Check des infos
                if (url.IsNullOrEmpty() || page.IsNull() || xpath.IsNull() ||
                    xpath.Titre.IsEmpty() || xpath.Href.IsEmpty()) return;
                //Get le lien
                string newUrl = null;
                // ReSharper disable once AccessToModifiedClosure
                foreach (var attribute in from href in xpath.Href.TakeWhile(href => !newUrl.IsNotNullOrEmpty())
                    let ul = page.DocumentNode.SelectSingleNode(href.Chemin)
                    where !ul.IsNull()
                    select ul.Attributes[href.Attribute]
                    into attribute
                    where !attribute.IsNull()
                    select attribute)
                {
                    //Get la valeur du href
                    newUrl = attribute.Value;
                }

                ////On regarde si l'url est déjjà présente
                //if (newUrl.InArray(_oldUrl)) return;

                if (newUrl.IsNull())
                {
                    //OnErrorGetElementPage(new[]
                    //{
                    //    "Impossible de récup le lien href",
                    //    "L'url de base est : " + xpath.UrlBase,
                    //    "L'url de l'article est : " + url
                    //});
                    return;
                }

                //Get du titre
                string titre = null;
                foreach (
                    var nodesTitre in
                        xpath.Titre.TakeWhile(xpathTitre => !titre.IsNotNullOrEmpty())
                            .Select(xpathTitre => page.DocumentNode.SelectNodes(xpathTitre.Chemin))
                            .Where(nodesTitre => !nodesTitre.IsNull()))
                {
                    foreach (var nodeTitre in nodesTitre.Where(nodeTitre => !nodeTitre.IsNull()))
                    {
                        titre = nodeTitre.InnerHtml.HtmlDecode().Replace(new[]
                        {
                            new Replace {OldChar = "<b>", NewChar = string.Empty},
                            new Replace {OldChar = "</b>", NewChar = string.Empty},
                            new Replace {OldChar = "<em>", NewChar = string.Empty},
                            new Replace {OldChar = "</em>", NewChar = string.Empty}
                        });
                        break;
                    }
                }
                if (titre.IsNullOrEmpty())
                {
                    //OnErrorGetElementPage(new[]
                    //{
                    //    "Impossible de récup le titre",
                    //    "L'url de base est : " + xpath.UrlBase,
                    //    "L'url de l'article est : " + url
                    //});
                    return;
                }

                //Get de l'image
                string srcImage = null;
                foreach (var img in xpath.Image)
                {
                    if (srcImage.IsNotNullOrEmpty()) break;
                    if (img.IsNull()) continue;
                    var nodesImage = page.DocumentNode.SelectNodes(img.Chemin);
                    if (!nodesImage.IsNotNull()) continue;
                    foreach (var attributImage in
                        nodesImage.Where(nodeImage => !nodeImage.IsNull())
                            .Select(nodeImage => nodeImage.Attributes[img.Attribute])
                            .Where(attributImage => attributImage.IsNotNull()))
                    {
                        srcImage = attributImage.Value;
                        break;
                    }
                }
                //Chargement de la page de l'article
                newUrl = newUrl.AddHost(url);
                var article = new Article
                {
                    Url = newUrl,
                    Titre = titre,
                    Params = xpath.Params,
                    ContenuArticle = null,
                    Image = srcImage
                };

                page = newUrl.RloadUrl();
                if (page.IsNull())
                {
                    //OnErrorGetElementPage(new[]
                    //{
                    //    "La page de l'article n'as pu être chargé",
                    //    "L'url de base est : " + xpath.UrlBase,
                    //    "L'url est : " + newUrl
                    //});
                    return;
                }

                foreach (
                    var para in
                        from contenuxpath in
                            xpath.ContenuArticle.TakeWhile(contenuxpath => !article.ContenuArticle.IsNotNullOrEmpty())
                        select page.DocumentNode.SelectNodes(contenuxpath.Chemin)
                        into listeParagraphe
                        where !listeParagraphe.IsNull()
                        from para in listeParagraphe
                        select para)
                {
                    //Suppression xpath <script> et commentaire web
                    para.InnerHtml = para.InnerHtml.DeleteJsWeb();
                    para.InnerHtml = para.InnerHtml.DeleteCommentaireWeb();
                    //Suppression des espaces
                    var temp = para.InnerText.EnleverEspaceMultiple();
                    //Suppression des balises
                    temp = temp.EnleverChainePerso("<script", "script>");
                    //Suppression retour chariot
                    temp = temp.DeleteRetourChariot();
                    if (temp.IsNullOrEmpty() || temp.Equals("&nbsp;")) continue;
                    article.ContenuArticle += temp.Replace(new[]
                    {
                        new Replace {OldChar = "<b>", NewChar = string.Empty},
                        new Replace {OldChar = "</b>", NewChar = string.Empty},
                        new Replace {OldChar = "<em>", NewChar = string.Empty},
                        new Replace {OldChar = "</em>", NewChar = string.Empty},
                        new Replace {OldChar = "\t", NewChar = string.Empty}
                    });
                    article.ContenuArticle += "<br/><br/>";
                }

                if (article.ContenuArticle.IsNullOrEmpty())
                {
                    //OnErrorGetElementPage(new[]
                    //{
                    //    "Erreur lors de la récupération du contenu de l'article",
                    //    "L'url de base est : " + xpath.UrlBase,
                    //    "L'url est : " + newUrl
                    //});
                    return;
                }


                //On cherche l'image si elle n'a pas été trouvée avant
                string imgSeconde = null;
                foreach (var img in xpath.Image)
                {
                    if (img.IsNull()) continue;
                    var nodesImage = page.DocumentNode.SelectNodes(img.Chemin);
                    if (!nodesImage.IsNotNull()) continue;
                    if (imgSeconde.IsNotNullOrEmpty()) break;
                    foreach (var attributImage in nodesImage.Where(nodeImage => !nodeImage.IsNull())
                        .Select(nodeImage => nodeImage.Attributes[img.Attribute])
                        .Where(attributImage => attributImage.IsNotNull())
                        .Where(attributImage => !attributImage.Value.Contains(new[] {"assets.pinterest.com"})))
                    {
                        imgSeconde = attributImage.Value;
                        break;
                    }
                }
                //On check l'image
                if (imgSeconde.IsNotNullOrEmpty())
                    article.Image = imgSeconde;

                //Check de l'url de l'image
                if (article.Image.IsNotNullOrEmpty())
                    article.Image = article.Image.AddHost(article.Url);

                //Notification
                OnNewArticle(article);
                //_oldUrl.Add(newUrl);
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
               // OnErrorGetElementPage(new[] {e.Message});
            }
        }
    }
}