using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
namespace R
{
    public static class RView
    {
        /// <summary>
        /// Converti le rendu d'une Vue en string
        /// </summary>
        /// <param name="partialView">La vue</param>
        /// <returns>string</returns>
        public static string RenderToString(this PartialViewResult partialView)
        {
            var httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                throw new NotSupportedException("La vue partiel est null");
            }

            var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();

            var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, controllerName);

            var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    view.Render(new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
                }
            }

            return sb.ToString();
        }

        public static string GetIdentityName(this HtmlHelper helper)
        {
            try
            {
                return helper.ViewContext.HttpContext.User.Identity.Name.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet d'afficher une image via son url
        /// </summary>
        /// <param name="html"></param>
        /// <param name="path">L'url dez l'image</param>
        /// <param name="width">Sa largeur</param>
        /// <param name="height">Sa hauteur</param>
        /// <returns>MvcHtmlString</returns>
        public static string Image(this Controller html, string path = null, string type = null)
        {
            return ExecImage(path, type);
        }

        private static string ExecImage(string path = null, string type = null)
        {
            try
            {
                if (path.IsNullOrEmpty() || type.IsNullOrEmpty()) return null;
                byte[] imageArray = RFichier.GetByte(path);
                if (imageArray.IsNull()) return null;
                var img = String.Format("data:" + type + ",{0}", Convert.ToBase64String(imageArray));
                if (img.IsNullOrEmpty()) return null;
                return img;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GenerateLienMvc(this HtmlHelper html, string chemin)
        {
            try
            {
                if (html.IsNull() || chemin.IsNullOrEmpty()) return chemin;
                var f = new UrlHelper(html.ViewContext.RequestContext);
                return f.Content(chemin);

            }
            catch (Exception)
            {
                return chemin;
            }
        }

        public static string GenerateFormAjax(this HtmlHelper html)
        {
            try
            {
                return "<p>" +
                       "<input type='text' name='nom' placeholder='Nom' class='inscriptionNom' required/>" +
                       "</p>" +
                       "<p>" +
                       "<input type='text' name='prenom' placeholder='Prénom' class='inscriptionPrenom' required/>" +
                       "</p>" +
                       "<p>" +
                       "<input type='date' name='date' placeholder='Date' class='inscriptionDate' required/>" +
                       "</p>" +
                       "<p>" +
                       "<input type='radio' class='inscriptionGenre' name='inscriptionGenre' value='0' checked='checked' />" +
                       "<label>Homme</label>" +
                       "<input type='radio' class='inscriptionGenre' name='inscriptionGenre' value='1' />" +
                       "<label>Femme</label>" +
                       "</p>" +
                       "<p>" +
                       "<input type='text' name='email' placeholder='Email' class='inscriptionEmail' required/>" +
                       "</p>" +
                       "<p>" +
                       "<input type='password' name='password' placeholder='Mot de passe' class='inscriptionPassword' required/>" +
                       "</p>" +
                       "<p>" +
                       "<input type='submit' name='submit' value='S&#146;inscrire' class='inscriptionClient' />" +
                       "</p>";
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de récupérer le nom du controller courant
        /// </summary>
        /// <param name="helper"></param>
        /// <returns>string</returns>
        public static string GetNameController(this HtmlHelper helper)
        {
            try
            {
                return HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de savoir si on est dans le controller via le nom passé en paramètre
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="nameController"></param>
        /// <returns></returns>
        public static bool IsController(this HtmlHelper helper, string nameController = null)
        {
            try
            {
                if (nameController.IsNullOrEmpty()) return false;

                return helper.GetNameController().ToLower().Equals(nameController.ToLower());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static T GetParameter<T>(this HtmlHelper helper, string nameParameter = null)
        {
            try
            {
                if (nameParameter.IsNullOrEmpty()) return default(T);
                var x = helper.ViewContext.RouteData.Values[nameParameter];
                return (T)Convert.ChangeType(x, typeof(T)); ;
            }
            catch (Exception e)
            {
                var g = e.Message;
                return default(T);
            }
        }
    }
}
