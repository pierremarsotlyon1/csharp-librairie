using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace R.Web
{
    public static class RequeteHttp
    {
        public delegate void DownloadFile();
        public static event Action<HtmlDocument, string, XpathArticle> RLoadPageCompleted;
        public static event Action<string> ErrorLoadingPage;

        private static void OnErrorLoadingPage(string obj)
        {
            Action<string> handler = ErrorLoadingPage;
            if (handler != null) handler(obj);
        }

        private static void OnRLoadPageCompleted(HtmlDocument obj, string url, XpathArticle xpath)
        {
            Action<HtmlDocument, string, XpathArticle> handler = RLoadPageCompleted;
            if (handler != null) handler(obj, url, xpath);
        }

        /// <summary>
        ///     Permet d'uploader des paramétres via une url
        /// </summary>
        /// <param name="str"></param>
        /// <param name="g">Objet</param>
        public static void UploadStringAsync(this string str, RGestionHttp g)
        {
            if (g.TabField.Length == 0) return;

            //Création du string des paramétres
            List<RFieldMethodeHttp> liste = g.TabField.ToList();
            string param = liste.Aggregate<RFieldMethodeHttp, string>(null,
                (current, field) =>
                    current +
                    (!liste.CheckLastElement(field)
                        ? field.Name + "=" + field.Value + "&"
                        : field.Name + "=" + field.Value));

            //Création de la requête http
            var webClient = GetWebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            if (param != null) webClient.UploadStringAsync(str.ToUri(), g.Methode, param);
        }

        /// <summary>
        ///     Permet d'uploader des paramétres via une url
        /// </summary>
        /// <param name="str"></param>
        /// <param name="g">Objet</param>
        public static string UploadStringSync(this string str, RGestionHttp g)
        {
            if (g.TabField.Length == 0) return null;

            //Création du string des paramétres
            List<RFieldMethodeHttp> liste = g.TabField.ToList();
            string param = liste.Aggregate<RFieldMethodeHttp, string>(null,
                (current, field) =>
                    current +
                    (!liste.CheckLastElement(field)
                        ? field.Name + "=" + field.Value + "&"
                        : field.Name + "=" + field.Value));

            //Création de la requête http
            var webClient = GetWebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            return param != null ? webClient.UploadString(str, g.Methode, param) : null;
        }

        /// <summary>
        ///     Permet de télécharger un fichier via une url de façon asynchrone
        /// </summary>
        /// <param name="str"></param>
        /// <param name="destination">Fichier de destination</param>
        /// <param name="d">Delegate lors de l'event</param>
        /// <returns>bool</returns>
        public static bool DownloadFileAsync(this string str, string destination, DownloadFile d)
        {
            try
            {
                //On tente de convertir le string en Uri
                Uri url = str.ToUri();
                if (url == null) return false;
                //Si la convertion a marché, on ajoute l'event et on télécharge
                var webClient = GetWebClient();
                webClient.DownloadFileCompleted += MyDownloadFileCompleted(ref d);
                webClient.DownloadFileAsync(url, destination);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de télécharger un fichier via une url de façon synchrone
        /// </summary>
        /// <param name="str"></param>
        /// <param name="destination">Fichier de destination</param>
        /// <returns>bool</returns>
        public static bool DownloadFileSynch(this string str, string destination)
        {
            try
            {
                //On supprime le // en début d'url
                while (str[0] == '/')
                {
                    str = str.RSubstring(1, str.Length-1);
                }

                //On tente de convertir le string en Uri
                Uri url = str.ToUri();
                if (url == null) return false;
                //Si la convertion a marché, on ajoute l'event et on télécharge
                var webClient = GetWebClient();
                webClient.DownloadFile(url, destination);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de charger le contenu d'une page de façon asynchrone
        /// </summary>
        /// <param name="url">L'url à charger</param>
        /// <param name="xpath"></param>
        public static bool RloadUrlAsynch(this string url, XpathArticle xpath)
        {
            try
            {
                RThread.DoAsync(() =>
                {
                    var web = GetHtmlWeb();
                    HtmlDocument document = web.Load(url);
                    OnRLoadPageCompleted(document, url, xpath);
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de charger le contenu d'une page de façon synchrone
        /// </summary>
        /// <param name="url">L'url à charger</param>
        /// <param name="xpath"></param>
        public static bool RloadUrl(this string url, XpathArticle xpath)
        {
            try
            {
                if (url.IsNullOrEmpty() || xpath.IsNull() || !url.IsUri())
                {
                    OnErrorLoadingPage("L'url n'est pas valide");
                    return false;
                }
                var client = GetWebClient();
                string reply = client.DownloadString(url);
                var document = new HtmlDocument();
                document.LoadHtml(reply);

                OnRLoadPageCompleted(document, url, xpath);

                return true;
            }
            catch (Exception)
            {
                try
                {
                    var document = GetHtmlWeb();
                    var x = document.Load(url);
                    if (x.IsNull()) return false;
                    OnRLoadPageCompleted(x, url, xpath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permet de charger le contenu d'une page de façon synchrone sans event
        /// </summary>
        /// <param name="url">L'url à charger</param>
        public static HtmlDocument RloadUrl(this string url)
        {
            try
            {
                if (url.IsNullOrEmpty() || !url.IsUri())
                {
                    OnErrorLoadingPage("L'url n'est pas valide");
                    return null;
                }
                var client = GetWebClient();
                
                string reply = client.DownloadString(url);
                client.Dispose();
                var document = new HtmlDocument();
                document.LoadHtml(reply);
                return document;
            }
            catch (Exception)
            {
                try
                {
                    var document = GetHtmlWeb();
                    var x = document.Load(url);
                    return x.IsNotNull() ? x : null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///     Event DownloadFileCompleted
        /// </summary>
        /// <param name="downloadFile">Delegate</param>
        /// <returns></returns>
        private static AsyncCompletedEventHandler MyDownloadFileCompleted(ref DownloadFile downloadFile)
        {
            downloadFile();
            return null;
        }

        private static HtmlWeb GetHtmlWeb()
        {
            return new HtmlWeb { OverrideEncoding = Encoding.UTF8 };
        }

        public static WebClient GetWebClient()
        {
            return new WebClient { Encoding = Encoding.UTF8 };
        }
    }

    public class RGestionHttp
    {
        public string Methode { get; set; }
        public RFieldMethodeHttp[] TabField { get; set; }
    }

    public class RFieldMethodeHttp : RFieldBalise
    {
    }
}