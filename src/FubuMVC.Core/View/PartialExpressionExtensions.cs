using System.Web;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

// LEAVE THE NAMESPACE ALONE PLEASE
namespace FubuMVC.Core.UI
{
    public static class PartialExpressionExtensions
    {
        public static IHtmlString Partial<TInputModel>(this IFubuPage page, string categoryOrHttpMethod = null) where TInputModel : class
        {
            return new HtmlString(InvokePartial<TInputModel>(page, null, categoryOrHttpMethod));
        }

        public static IHtmlString PartialFor(this IFubuPage page, object input, bool withModelBinding = false, string categoryOrHttpMethod = null)
        {
            return new HtmlString(page.Get<IPartialInvoker>().InvokeObject(input, withModelBinding, categoryOrHttpMethod));
        }

        public static IHtmlString Partial<TInputModel>(this IFubuPage page, TInputModel model, bool withModelBinding = false, string categoryOrHttpMethod = null) where TInputModel : class
        {
            if (typeof(TInputModel) == typeof(object))
            {
                return new HtmlString(page.Get<IPartialInvoker>().InvokeObject(model, withModelBinding, categoryOrHttpMethod));
            }

            page.Get<IFubuRequest>().Set(model);
            return new HtmlString(InvokePartial<TInputModel>(page, categoryOrHttpMethod));
        }

        public static string InvokePartial<TInputModel>(IFubuPage page, string prefix, string categoryOrHttpMethod = null) where TInputModel : class
        {
            return page.Get<IPartialInvoker>().Invoke<TInputModel>(categoryOrHttpMethod);
        }
    }
}