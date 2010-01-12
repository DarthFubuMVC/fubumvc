using System;

namespace FubuMVC.Core.View
{
    public interface IViewEngine<T> where T : class
    {
        void RenderView(ViewPath viewPath, Action<T> configureView);
    }
}