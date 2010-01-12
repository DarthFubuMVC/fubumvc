using System;
using System.Web.UI;

namespace FubuMVC.Core.View.WebForms
{
    public class WebFormViewEngine<T> : IViewEngine<T> where T : class
    {
        private readonly IWebFormsControlBuilder _builder;
        private readonly IWebFormRenderer _renderer;

        public WebFormViewEngine(IWebFormsControlBuilder builder, IWebFormRenderer renderer)
        {
            _builder = builder;
            _renderer = renderer;
        }

        public void RenderView(ViewPath viewPath, Action<T> configureView)
        {
            Control control = _builder.LoadControlFromVirtualPath(viewPath.ViewName, typeof (T));
            var view = control as T;
            if (view != null)
            {
                configureView(view);
            }

            _renderer.RenderControl(control);
        }
    }
}