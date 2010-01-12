using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;

namespace FubuMVC.Core.Behaviors
{
    public class RenderFubuWebFormView : RenderFubuViewBehavior
    {
        public RenderFubuWebFormView(WebFormViewEngine<IFubuView> engine, IFubuRequest request, ViewPath view, IViewActivator activator)
            : base(engine, request, view, activator)
        {
        }
    }
}