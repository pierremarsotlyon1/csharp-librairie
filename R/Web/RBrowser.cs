using System;
using System.Text.RegularExpressions;
using System.Web;

namespace R.Web
{
    public static class RBrowser
    {
        /// <summary>
        /// Obtient le type de browser utilisé
        /// </summary>
        /// <returns>string</returns>
        public static string GetBrowser()
        {
            try
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtient la langue du browser
        /// </summary>
        /// <returns>string</returns>
        public static string GetLanguageBrowser()
        {
            try
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"].Split(',')[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de vérifier si le client vient d'un mobile
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsMobile()
        {
            try
            {
                return HttpContext.Current.Request.Browser.IsMobileDevice;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
