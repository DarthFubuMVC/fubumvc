using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.WebForms
{
    public class WebFormsRenderableView : IRenderableView
    {
        // TODO -- consider just moving this into FubuPage / FubuControl / what have you

        private readonly IFubuPage _inner;
        private readonly IWebFormRenderer _renderer;
        private readonly Control _control;

        public WebFormsRenderableView(IWebFormRenderer renderer, Control control)
        {
            _renderer = renderer;
            _control = control;
            _inner = (IFubuPage)_control;
        }

        public string ElementPrefix
        {
            get { return _inner.ElementPrefix; }
            set { _inner.ElementPrefix = value; }
        }

        public IServiceLocator ServiceLocator
        {
            get { return _inner.ServiceLocator; }
            set { _inner.ServiceLocator = value; }
        }

        public IUrlRegistry Urls
        {
            get { return _inner.Urls; }
        }

        public T Get<T>()
        {
            return _inner.Get<T>();
        }

        public T GetNew<T>()
        {
            return _inner.GetNew<T>();
        }

        public void Write(object content)
        {
            _inner.Write(content);
        }

        public void Render()
        {
            _renderer.RenderControl(_control);
        }
    }
}