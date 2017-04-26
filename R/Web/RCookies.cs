using System;
using System.Web;

namespace R
{
    public static class RCookies
    {
        /// <summary>
        /// Vérification de l'autorisation des Cookies
        /// </summary>
        /// <returns>bool</returns>
        public static bool AutorisationCookies()
        {
            return HttpContext.Current.Request.Browser.Cookies;
        }

        /// <summary>
        /// Ajoute un Cookie
        /// </summary>
        /// <param name="key">Nom du cookie</param>
        /// <param name="value">Valeur du cookie</param>
        /// <param name="temps">Temps d'activation du cookie en jours</param>
        /// <returns>bool</returns>
        public static bool AddCookies(string key = null, string value = null, int temps = 700)
        {
            try
            {
                if (key.IsNullOrEmpty() || value.IsNullOrEmpty()) return false;

                HttpCookie myCookie = GetRequestCookie(key);
                if (myCookie.IsNull())
                {
                    //On ajoute le cookie
                    myCookie = new HttpCookie(key)
                    {
                        Value = value,
                        Expires = DateTime.Now.AddDays(temps)
                    };

                    HttpContext.Current.Response.Cookies.Add(myCookie);
                }
                else
                {
                    //On update le cookie
                    myCookie.Value = value;
                    myCookie.Expires = DateTime.Now.AddDays(temps);

                    return UpdateCookies(myCookie);
                }
                
                return true;
            }
            catch (Exception e)
            {
                string h = e.Message;
                return false;
            }
        }

        private static bool UpdateCookies(HttpCookie myCookie = null)
        {
            try
            {
                if (myCookie.IsNull()) return false;

                HttpContext.Current.Response.Cookies.Set(myCookie);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateCookies(string key = null, string value = null, int temps = 700)
        {
            try
            {
                if (key.IsNullOrEmpty() || value.IsNullOrEmpty()) return false;

                HttpCookie myCookie = GetRequestCookie(key);
                if (myCookie.IsNull())
                    return AddCookies(key, value, temps);

                myCookie.Value = value;
                myCookie.Expires = DateTime.Now.AddDays(temps);

                return UpdateCookies(myCookie);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Récupére un cookie
        /// </summary>
        /// <param name="key">Nom du cookie</param>
        /// <returns>string</returns>
        public static string GetCookie(string key = null)
        {
            try
            {
                if (key.IsNullOrEmpty()) return null;

                HttpCookie myCookie = GetRequestCookie(key);
                if (myCookie.IsNull() || myCookie.Value.IsNullOrEmpty()) return null;
                return myCookie.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Supprime un cookie
        /// </summary>
        /// <param name="key">Nom du cookie</param>
        /// <returns>bool</returns>
        public static bool DeleteCookie(string key = null)
        {
            try
            {
                if (key.IsNullOrEmpty()) return false;

                var cookie = GetRequestCookie(key);
                if (cookie.IsNull()) return false;

                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime tout les cookies
        /// </summary>
        /// <returns>bool</returns>
        public static bool ClearCookies()
        {
            try
            {
                HttpContext.Current.Request.Cookies.Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Récupère la "Session" Cookie
        /// </summary>
        /// <param name="key">Nom du groupe dans lequel les cookies se trouvent</param>
        /// <returns>HttpCookie</returns>
        private static HttpCookie GetRequestCookie(string key = null)
        {
            try
            {
                if (key.IsNullOrEmpty()) return null;

                return HttpContext.Current.Request.Cookies.Get(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Récupère la "Response" Cookie
        /// </summary>
        /// <returns>HttpCookieCollection</returns>
        private static HttpCookieCollection GetResponseCookie()
        {
            return HttpContext.Current.Response.Cookies;
        }
    }
}
