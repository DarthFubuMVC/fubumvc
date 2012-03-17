using System;

namespace FubuMVC.Core.View.Rendering
{
    public static class FubuPageRenderingExtensions
    {
        public static T Modify<T>(this T view, Action<IFubuPage> modify) where T : IRenderableView
        {
            modify(view);
            return view;
        }
    }
}