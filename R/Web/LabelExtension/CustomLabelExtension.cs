using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace R
{
    public static class CustomLabelExtension
    {
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string resolvedLabelText = metadata.DisplayName ?? metadata.PropertyName;
            if (metadata.IsRequired)
            {
                resolvedLabelText += " *";
            }

            return LabelExtensions.LabelFor(html, expression, resolvedLabelText);
        }
    }
}
