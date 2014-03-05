using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using Spark;

namespace FubuMVC.Spark.Rendering
{
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

        public void Render(IFubuRequestContext context)
        {
            PreRender.Do(_view);
            _view.Render(context);
            PostRender.Do(_view);
        }

        public IFubuPage Page
        {
            get { return _view; }
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