using System;
using System.IO;
using System.Web;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using HtmlTags;

using Spark;
using System.Collections.Generic;

namespace FubuMVC.Spark.Rendering
{
    public abstract class FubuSparkView : SparkViewBase, IFubuSparkView
    {
        private readonly Cache<Type, object> _services = new Cache<Type, object>();

        protected FubuSparkView()
        {
            _services.OnMissing = type => ServiceLocator.GetInstance(type);
        }

        public IServiceLocator ServiceLocator { get; set; }

        public T Get<T>()
        {
            return (T)_services[typeof(T)];
        }

        public T GetNew<T>()
        {
            return (T)ServiceLocator.GetInstance(typeof(T));
        }

        public void Write(object content)
        {
            Output.Write(content);
        }

        public IUrlRegistry Urls
        {
            get { return Get<IUrlRegistry>(); }
        }

        public Func<string, string> SiteResource { get; set; }

        string IFubuPage.ElementPrefix { get; set; }

        public HtmlTag Tag(string tagName)
        {
            return new HtmlTag(tagName);
        }

        public string H(object value)
        {
            return Get<IHtmlEncoder>().Encode(value);
        }

        public HtmlString HTML(object value)
        {
            return new HtmlString(value != null ? value.ToString() : null);
        }
    }

    public abstract class FubuSparkView<TViewModel> : FubuSparkView, IFubuPage<TViewModel> where TViewModel : class
    {
        public void SetModel(IFubuRequest request)
        {
            Model = request.Get<TViewModel>();
        }

        public void SetModel(object model)
        {
            SetModel(model as TViewModel);
        }

        public void SetModel(TViewModel model)
        {
            Model = model;
        }

        public TViewModel Model { get; set; }

        public object GetModel()
        {
            return Model;
        }
    }

    public interface IFubuSparkView : IRenderableView
    {
        Dictionary<string, TextWriter> Content { set; get; }
        Dictionary<string, string> OnceTable { set; get; }
        Dictionary<string, object> Globals { set; get; }
        TextWriter Output { get; set; }
        
        Guid GeneratedViewId { get; }

        ICacheService CacheService { get; set; }
        Func<string, string> SiteResource { get; set; }
    }

    public static class FubuSparkViewExtensions
    {
        public static IFubuSparkView Modify(this IFubuSparkView view, Action<IFubuSparkView> modify)
        {
            modify(view);
            return view;
        }
    }

    public class FubuSparkViewDecorator : IFubuSparkView
    {
        private readonly IFubuSparkView _view;
        public FubuSparkViewDecorator(IFubuSparkView view)
        {
            _view = view;
            PreRender = new CompositeAction<IFubuSparkView>();
            PostRender = new CompositeAction<IFubuSparkView>();
        }

        public CompositeAction<IFubuSparkView> PreRender { get; set; }
        public CompositeAction<IFubuSparkView> PostRender { get; set; }

        public void Render()
        {
            PreRender.Do(_view);
            _view.Render();
            PostRender.Do(_view);
        }

        public Guid GeneratedViewId
        {
            get { return _view.GeneratedViewId; }
        }

        public ICacheService CacheService
        {
            get { return _view.CacheService; }
            set { _view.CacheService = value; }
        }

        public Func<string, string> SiteResource
        {
            get { return _view.SiteResource; }
            set { _view.SiteResource = value; }
        }

        public Dictionary<string, TextWriter> Content
        {
            get { return _view.Content; }
            set { _view.Content = value; }
        }

        public Dictionary<string, string> OnceTable
        {
            get { return _view.OnceTable; }
            set { _view.OnceTable = value; }
        }

        public Dictionary<string, object> Globals
        {
            get { return _view.Globals; }
            set { _view.Globals = value; }
        }

        public TextWriter Output
        {
            get { return _view.Output; }
            set { _view.Output = value; }
        }

        public string ElementPrefix
        {
            get { return _view.ElementPrefix; }
            set { _view.ElementPrefix = value; }
        }

        public IServiceLocator ServiceLocator
        {
            get { return _view.ServiceLocator; }
            set { _view.ServiceLocator = value; }
        }

        public IUrlRegistry Urls
        {
            get { return _view.Urls; }
        }

        public T Get<T>()
        {
            return _view.Get<T>();
        }

        public T GetNew<T>()
        {
            return _view.GetNew<T>();
        }

        public void Write(object content)
        {
            _view.Write(content);
        }
    }
}