using System;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.UI.ViewEngine
{
    public class HtmlDocumentViewFactory<T> : IViewFactory, IRenderableView where T : FubuHtmlDocument
    {
        private readonly IOutputWriter _writer;
        private readonly Lazy<T> _document; 

        public HtmlDocumentViewFactory(IOutputWriter writer, IServiceLocator services)
        {
            _writer = writer;
            _document = new Lazy<T>(() => services.GetInstance<T>());
        }

        public void Render()
        {
            _writer.Write(MimeType.Html, _document.Value.ToString());
        }

        public IFubuPage Page
        {
            get { return _document.Value; }
        }

        public void Describe(Description description)
        {
            description.Title = typeof (T).Name;
        }

        public IRenderableView GetView()
        {
            return this;
        }

        public IRenderableView GetPartialView()
        {
            return this;
        }
    }
}