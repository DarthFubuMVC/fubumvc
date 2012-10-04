using System;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using FubuCore;

namespace FubuMVC.Core.UI.ViewEngine
{
    public class HtmlDocumentViewFactory<T> : IViewFactory, IRenderableView where T : FubuHtmlDocument
    {
        private readonly IOutputWriter _writer;
        private readonly IDocumentHolder<T> _proxy;


        public HtmlDocumentViewFactory(IOutputWriter writer, IServiceLocator services)
        {
            _writer = writer;

            var modelType = FubuCore.TypeExtensions.FindParameterTypeTo(typeof (T), typeof (IFubuPage<>));

            _proxy = typeof (FubuPageProxy<,>).CloseAndBuildAs<IDocumentHolder<T>>(typeof(T),modelType);
            _proxy.ServiceLocator = services;
        }

        public void Render()
        {
            _writer.Write(MimeType.Html, _proxy.Document.ToString());
        }

        public IFubuPage Page
        {
            get { return _proxy; }
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

    public interface IDocumentHolder<T> : IFubuPage
    {
        T Document { get; }
    }

    public class FubuPageProxy<T, TModel> : IFubuPage<TModel>, IDocumentHolder<T> where TModel : class where T : FubuHtmlDocument<TModel>
    {
        private readonly Lazy<T> _document;

        public FubuPageProxy()
        {
            _document = new Lazy<T>(() =>
            {
                var doc = ServiceLocator.GetInstance<T>();
                doc.ElementPrefix = ElementPrefix;


                return doc;
            });
        }

        public T Document
        {
            get { return _document.Value; }
        }

        public string ElementPrefix { get; set; }
        public IServiceLocator ServiceLocator { get; set; }
        public IUrlRegistry Urls { get; private set; }
        public TService Get<TService>()
        {
            return ServiceLocator.GetInstance<TService>();
        }

        public T GetNew<T>()
        {
            throw new NotImplementedException();
        }

        public void Write(object content)
        {
            throw new NotImplementedException();
        }

        public object GetModel()
        {
            return Model;
        }

        public TModel Model { get; set; }
    }
}