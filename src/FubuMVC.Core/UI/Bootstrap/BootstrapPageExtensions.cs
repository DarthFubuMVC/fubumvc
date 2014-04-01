using FubuMVC.Core.Assets;
using FubuMVC.Core.UI.Bootstrap.Collapsibles;
using FubuMVC.Core.UI.Bootstrap.Modals;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.Bootstrap
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