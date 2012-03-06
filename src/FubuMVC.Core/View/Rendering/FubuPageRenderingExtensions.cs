using System;

namespace FubuMVC.Core.View.Rendering
{
    public static class FubuPageRenderingExtensions
    {
        public static IRenderableView Modify(this IRenderableView view, Action<IFubuPage> modify)
        {
            modify(view);
            return view;
        }
    }
}