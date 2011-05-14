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
    public abstract class FubuSparkView : SparkViewBase, IFubuPage, IFubuSparkView
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

    public interface IFubuSparkView : ISparkView
    {
		// Try to hide this away
		Dictionary<string, TextWriter> Content { set; get; }
		Dictionary<string, string> OnceTable { set; get; }
        TextWriter Output { get; set; }
     
		Func<string, string> SiteResource { get; set; }
    }
}