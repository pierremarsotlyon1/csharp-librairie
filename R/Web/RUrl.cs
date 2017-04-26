using System;
using System.Linq;
using System.Web;

// ReSharper disable once CheckNamespace
namespace R
{
    public static class RUrl
    {
        /// <summary>
        /// Obtient l'url absolute ( sans le domaine ) de la page précédente
        /// </summary>
        /// <returns>string</returns>
        public static string GetUrlAbsolutePathPageAppellante()
        {
            return HttpContext.Current.Request.UrlReferrer != null ? HttpContext.Current.Request.UrlReferrer.AbsolutePath : null;
        }

        public static Uri GetReferrer()
        {
            try
            {
                return HttpContext.Current.Request.UrlReferrer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtient l'url relative avec le domaine de la page précédente
        /// </summary>
        /// <returns>string</returns>
        public static string GetUrlAbsoluteUriPageAppellante()
        {
            return HttpContext.Current.Request.UrlReferrer != null ? HttpContext.Current.Request.UrlReferrer.AbsoluteUri : null;
        }

        /// <summary>
        /// Obtient l'url absolute ( sans le domaine ) de la page courante
        /// </summary>
        /// <returns>string</returns>
        public static string GetUrlAbsolutePageCourante()
        {
            return HttpContext.Current.Request.Url.AbsolutePath;
        }

        /// <summary>
        /// Obtient l'url relative avec le domaine de la page courante
        /// </summary>
        /// <returns>string</returns>
        public static string GetUrlCompletePageCourante()
        {
            return HttpContext.Current.Request.Url.AbsoluteUri;
        }

        /// <summary>
        /// Permet de convertir un string en Uri
        /// </summary>
        /// <param name="url"></param>
        /// <param name="toTry"></param>
        /// <returns>Uri</returns>
        public static Uri ToUri(this string url, bool toTry = false)
        {
            try
            {
                return new Uri(url);
            }
            catch (Exception)
            {
                if (toTry) return null;
                var temp = ToUri("http://" + url, true);
                return temp;
            }
        }

        /// <summary>
        /// Vérifie si une url est valide
        /// </summary>
        /// <param name="url">L'url</param>
        /// <returns>bool</returns>
        public static bool IsUri(this string url)
        {
            try
            {
                Uri uri;
                var b = Uri.TryCreate(url, UriKind.Absolute, out uri) || uri.IsNull();
                return b;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet d'obtenir l'host d'une Uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>string</returns>
        public static string GetHostUri(this Uri uri)
        {
            try
            {
                return uri.Host;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string AddHost(this string urlCible, string urlbase)
        {
            try
            {
                var host = urlbase.ToUri().GetHostUri();
                if (urlCible.Contains(host) || urlCible.Contains("http://") ||
                    urlCible.Contains(GetTabExtensionDomaine())) return urlCible;
                urlCible = host + urlCible;
                if (!urlCible.Contains("http://"))
                {
                    urlCible = "http://" + urlCible;
                }
                return urlCible;
            }
            catch (Exception)
            {
                return urlCible;
            }
        }

        private static string[] GetTabExtensionDomaine()
        {
            return new[]
                {
                    ".fr", ".com", ".net", ".co", ".be", ".eu"
                };
        }

        public static string DeleteTokenUrlAsp(this string url)
        {
            try
            {
                if (url.IsNullOrEmpty()) return url;

                //On regarde si il y a le token (S(...))
                if (!url.Contains("(S(")) return url;

                var index = url.IndexOf("(S(", StringComparison.Ordinal);
                if (index < 0) return url;

                var indexFin = url.IndexOf(")/", index, StringComparison.Ordinal);
                return indexFin < 0 ? url : url.Remove(index, (indexFin+2) - index);
            }
            catch (Exception)
            {
                return url;
            }
        }

        public static string RemoveParameter(this string url, char key)
        {
            try
            {
                if (url.IsNullOrEmpty()) return url;
                
                //On regarde si l'url contient le key
                var index = url.IndexOf(key);
                if (index < 0) return url;

                return url.Split(key).FirstOrDefault();
            }
            catch (Exception)
            {
                return url;
            }
        }
    }
}
