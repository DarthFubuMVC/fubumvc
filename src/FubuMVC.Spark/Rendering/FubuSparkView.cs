using System;
using System.IO;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
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

    public interface IFubuSparkView : IFubuPage
    {
        Dictionary<string, TextWriter> Content { set; get; }
        Dictionary<string, string> OnceTable { set; get; }
        TextWriter Output { get; set; }
        void Render();
        Func<string, string> SiteResource { get; set; }
        Guid GeneratedViewId { get; }
    }

    public static class FubuSparkViewExtensions
    {
        public static IFubuSparkView Modify(this IFubuSparkView view, Action<IFubuSparkView> modify)
        {
            modify(view);
            return view;
        }
    }

    public class FubuSparkViewDecorator : FubuSparkView, IFubuSparkView
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

        public override void Render()
        {
            PreRender.Do(_view);
            _view.Render();
            PostRender.Do(_view);
        }

        public override Guid GeneratedViewId
        {
            get { return _view.GeneratedViewId; }
        }

        Func<string, string> IFubuSparkView.SiteResource
        {
            get { return _view.SiteResource; } 
            set { _view.SiteResource = value; }
        }

        Dictionary<string, TextWriter> IFubuSparkView.Content
        {
            get { return _view.Content; } 
            set { _view.Content = value; }
        }

        Dictionary<string, string> IFubuSparkView.OnceTable
        {
            get { return _view.OnceTable; } 
            set { _view.OnceTable = value; }
        }

        TextWriter IFubuSparkView.Output
        {
            get { return _view.Output; } 
            set { _view.Output = value; }
        }
    }
}