using FubuCore;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Razor.Rendering
{
    public class LayoutActivation : BasicViewModifier<IFubuRazorView>
    {
        public override IFubuRazorView Modify(IFubuRazorView view)
        {
            if (view.Layout != null)
            {
                view.Layout.As<IFubuRazorView>().ServiceLocator = view.ServiceLocator;
                Modify(view.Layout.As<IFubuRazorView>());
            }
            return view;
        }
    }
}