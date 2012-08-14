using System.Web;

using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI
{
    public static class PartialExpressionExtensions
    {
        public static IHtmlString Partial<TInputModel>(this IFubuPage page) where TInputModel : class
        {
            return new HtmlString(InvokePartial<TInputModel>(page, null));
        }

        public static IHtmlString PartialFor(this IFubuPage page, object input, bool withModelBinding = false)
        {
            return new HtmlString(page.Get<IPartialInvoker>().InvokeObject(input, withModelBinding));
        }

        public static IHtmlString Partial<TInputModel>(this IFubuPage page, TInputModel model, bool withModelBinding = false) where TInputModel : class
        {
            if (typeof(TInputModel) == typeof(object))
            {
                return new HtmlString(page.Get<IPartialInvoker>().InvokeObject(model, withModelBinding));
            }

            page.Get<IFubuRequest>().Set(model);
            return new HtmlString(InvokePartial<TInputModel>(page, null));
        }

        public static string InvokePartial<TInputModel>(IFubuPage page, string prefix) where TInputModel : class
        {
            return page.Get<IPartialInvoker>().Invoke<TInputModel>();
        }
    }
}