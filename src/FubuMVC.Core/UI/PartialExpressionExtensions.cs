using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI
{
    public static class PartialExpressionExtensions
    {
        public static void Partial<TInputModel>(this IFubuPage page) where TInputModel : class
        {
            InvokePartial<TInputModel>(page, null);
        }

        public static void PartialFor(this IFubuPage page, object input)
        {
            page.Get<PartialInvoker>().InvokeObject(input);
        }

        public static void Partial<TInputModel>(this IFubuPage page, TInputModel model) where TInputModel : class
        {
            if (typeof (TInputModel) == typeof (object))
            {
                page.Get<PartialInvoker>().InvokeObject(model);
            }
            else
            {
                page.Get<IFubuRequest>().Set(model);
                InvokePartial<TInputModel>(page, null);
            }
        }

        public static void InvokePartial<TInputModel>(IFubuPage page, string prefix) where TInputModel : class
        {
            page.Get<PartialInvoker>().Invoke<TInputModel>();
        }
    }
}