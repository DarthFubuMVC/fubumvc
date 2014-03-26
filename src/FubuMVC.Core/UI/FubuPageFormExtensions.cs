using System;
using System.Linq.Expressions;
using System.Web;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.View;
using HtmlTags;
using FubuCore;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI
{
    public static class FubuPageFormExtensions
    {
        public static HtmlTag FormFor(this IFubuPage page)
        {
            return new FormTag().NoClosingTag();
        }

        public static HtmlTag FormFor(this IFubuPage page, string url)
        {
            url = page.Get<IHttpRequest>().ToFullUrl(url);
            return new FormTag(url).NoClosingTag();
        }

        public static HtmlTag FormFor<TInputModel>(this IFubuPage page) where TInputModel : new()
        {
            var search = ChainSearch.ByUniqueInputType(typeof (TInputModel), "POST");
            return page.FormFor(search, new TInputModel());
        }

        public static HtmlTag FormFor<TInputModel>(this IFubuPage page, Action<HtmlTag> configure) where TInputModel : new()
        {
            var search = ChainSearch.ByUniqueInputType(typeof (TInputModel), "POST");
            return page.FormFor(search, new TInputModel(), configure);
        }

        public static HtmlTag FormFor<TInputModel>(this IFubuPage page, TInputModel model)
        {
            return page.FormFor(ChainSearch.ByUniqueInputType(model.GetType(), "POST"), model);
        }

        public static HtmlTag FormFor<TController>(this IFubuPage view, Expression<Action<TController>> expression)
        {
            var search = ChainSearch.ForMethod(expression, "POST");
            return view.FormFor(search, null);
        }

        public static HtmlTag FormFor(this IFubuPage view, object input)
        {
            if (input is string)
            {
                return view.FormFor(input.As<string>());
            }

            var search = ChainSearch.ByUniqueInputType(input.GetType(), "POST");
            return view.FormFor(search, input);
        }

        public static HtmlTag FormFor(this IFubuPage view, ChainSearch search, object input)
        {
            var request = new FormRequest(search, input);

            return view.Get<ITagGeneratorFactory>().GeneratorFor<FormRequest>().Build(request);
        }

        public static HtmlTag FormFor(this IFubuPage view, ChainSearch search, object input, Action<HtmlTag> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException("No form configuration provided", "configure");
            }

            var request = new FormRequest(search, input, true);
            var tag = view.Get<ITagGeneratorFactory>().GeneratorFor<FormRequest>().Build(request);
            configure(tag);
            return tag;
        }

        public static IHtmlString EndForm(this IFubuPage page)
        {
            return new HtmlString("</form>");
        }
    }
}