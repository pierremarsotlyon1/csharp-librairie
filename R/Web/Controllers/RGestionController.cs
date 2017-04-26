using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace R
{
    public static class HtmlRequestHelper
    {
        public static string Id(this HtmlHelper htmlHelper)
        {
            try
            {
                var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
                if (routeValues.IsEmpty()) return null;

                if (routeValues.ContainsKey("id"))
                    return (string)routeValues["id"];
                if (HttpContext.Current.Request.QueryString.AllKeys.Contains("id"))
                    return HttpContext.Current.Request.QueryString["id"];

                return string.Empty;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Controller(this HtmlHelper htmlHelper)
        {
            try
            {
                var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
                if (routeValues.IsEmpty()) return null;

                if (routeValues.ContainsKey("controller"))
                    return (string)routeValues["controller"];

                return string.Empty;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public static string Action(this HtmlHelper htmlHelper)
        {
            try
            {
                var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
                if (routeValues.IsEmpty()) return null;

                if (routeValues.ContainsKey("action"))
                    return (string)routeValues["action"];

                return string.Empty;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
