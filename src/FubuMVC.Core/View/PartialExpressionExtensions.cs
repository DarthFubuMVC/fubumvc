using System.Web;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.View
{
    public static class PartialExpressionExtensions
    {
        public static IHtmlString Partial<TInputModel>(this IFubuPage page, string categoryOrHttpMethod = null) where TInputModel : class
        {
            return new HtmlString(InvokePartial<TInputModel>(page, null, categoryOrHttpMethod));
        }

        public static IHtmlString PartialFor(this IFubuPage page, object input, bool withModelBinding = false, string categoryOrHttpMethod = null)
        {

            return new HtmlString(page.Get<IPartialInvoker>().InvokeAsHtml(input).GetAwaiter().GetResult());
        }

        public static IHtmlString Partial<TInputModel>(this IFubuPage page, TInputModel model, bool withModelBinding = false, string categoryOrHttpMethod = null) where TInputModel : class
        {
            if (typeof(TInputModel) == typeof(object))
            {
                return new HtmlString(page.Get<IPartialInvoker>().InvokeAsHtml(model).GetAwaiter().GetResult());
            }


            page.Get<IFubuRequest>().Set(model);
            return new HtmlString(InvokePartial<TInputModel>(page, categoryOrHttpMethod));
        }

        public static string InvokePartial<TInputModel>(IFubuPage page, string prefix, string categoryOrHttpMethod = null) where TInputModel : class
        {
            return page.Get<IPartialInvoker>().Invoke<TInputModel>(categoryOrHttpMethod).GetAwaiter().GetResult();
        }
    }
}