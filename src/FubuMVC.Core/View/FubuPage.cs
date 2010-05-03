using System;
using System.Web.UI;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View
{
    public class FubuPage<TViewModel> : FubuPage, IFubuPage<TViewModel> where TViewModel : class
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

        public TViewModel Model { get; private set; }
    }

    public class FubuPage : Page, IFubuPage
    {
        private readonly Cache<Type, object> _services = new Cache<Type, object>();

        public FubuPage()
        {
            _services.OnMissing = type => { return ServiceLocator.GetInstance(type); };
        }

        public IServiceLocator ServiceLocator { get; set; }

        public T Get<T>()
        {
            return (T) _services[typeof (T)];
        }

        public T GetNew<T>()
        {
            return ServiceLocator.GetInstance<T>();
        }

        public IUrlRegistry Urls
        {
            get { return Get<IUrlRegistry>(); }
        }

        string IFubuPage.ElementPrefix { get; set; }

        public HtmlTag Tag(string tagName)
        {
            return new HtmlTag(tagName);
        }
    }
}