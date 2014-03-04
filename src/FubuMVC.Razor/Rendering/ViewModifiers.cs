using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Razor.Rendering
{
    public class FubuPartialRendering : IViewModifier<IFubuRazorView>
    {
        private bool _shouldInvokeAsPartial;

        public bool Applies(IFubuRazorView view)
        {
            if (_shouldInvokeAsPartial)
            {
                return true;
            }

            _shouldInvokeAsPartial = true;
            return false;
        }

        public IFubuRazorView Modify(IFubuRazorView view)
        {
            view.NoLayout();
            return view;
        }
    }
}