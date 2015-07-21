using FubuMVC.Core.Assets;
using FubuMVC.Core.View.Bootstrap.Collapsibles;
using FubuMVC.Core.View.Bootstrap.Modals;

namespace FubuMVC.Core.View.Bootstrap
{
    public static class BootstrapPageExtensions
    {

        public static CollapsiblePartialExpression CollapsiblePartialFor<TInputModel>(this IFubuPage page) where TInputModel : class
        {
            page.Asset("twitter/activate-collapsible.js");
            return new CollapsiblePartialExpression(() => page.Partial<TInputModel>());
        }

        public static CollapsiblePartialExpression CollapsiblePartialFor(this IFubuPage page, object input)
        {
            page.Asset("twitter/activate-collapsible.js");
            return new CollapsiblePartialExpression(() => page.PartialFor(input));
        }

        public static CollapsiblePartialExpression CollapsiblePartialFor<TInputModel>(this IFubuPage page, TInputModel model) where TInputModel : class
        {
            page.Asset("twitter/activate-collapsible.js");
            return new CollapsiblePartialExpression(() => page.PartialFor(model));
        }

        public static ModalExpression Modal(this IFubuPage page, string id)
        {
            return new ModalExpression(page, id);
        }
    }
}