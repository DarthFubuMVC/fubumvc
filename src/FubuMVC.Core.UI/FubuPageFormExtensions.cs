using System;
using System.Linq.Expressions;
using FubuMVC.Core.Http;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public static class FubuPageFormExtensions
    {
        public static FormTag FormFor(this IFubuPage page)
        {
            return new FormTag();
        }

        public static FormTag FormFor(this IFubuPage page, string url)
        {
            url = page.Get<ICurrentHttpRequest>().ToFullUrl(url);
            return new FormTag(url);
        }

        public static FormTag FormFor<TInputModel>(this IFubuPage page) where TInputModel : new()
        {
            string url = page.Urls.UrlFor(new TInputModel(), categoryOrHttpMethod: "POST");
            return new FormTag(url);
        }

        public static FormTag FormFor<TInputModel>(this IFubuPage page, TInputModel model)
        {
            string url = page.Urls.UrlFor(model, categoryOrHttpMethod: "POST");
            return new FormTag(url);
        }


        public static FormTag FormFor<TController>(this IFubuPage view, Expression<Action<TController>> expression)
        {
            string url = view.Urls.UrlFor(expression, categoryOrHttpMethod: "POST");
            return new FormTag(url);
        }


        public static FormTag FormFor(this IFubuPage view, object modelOrUrl)
        {
            string url = modelOrUrl as string ?? view.Urls.UrlFor(modelOrUrl, categoryOrHttpMethod: "POST");

            return new FormTag(url);
        }

        public static string EndForm(this IFubuPage page)
        {
            return "</form>";
        }
    }
}