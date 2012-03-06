using FubuCore;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Razor.Rendering
{
    public class LayoutActivation : BasicViewModifier
    {
        public override bool Applies(IRenderableView view)
        {
            return view is IFubuRazorView;
        }

        public override IRenderableView Modify(IRenderableView view)
        {
            var razorView = view.As<IFubuRazorView>();
            if (razorView.Layout != null)
            {
                razorView.Layout.As<IFubuRazorView>().ServiceLocator = view.ServiceLocator;
                Modify(razorView.Layout.As<IFubuRazorView>());
            }
            return view;
        }
    }
}