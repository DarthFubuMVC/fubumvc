using System;
using System.Web.UI;
using FubuCore.Util;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.WebForms
{
    public class FubuPage<TViewModel> : FubuPage, IFubuPage<TViewModel> where TViewModel : class
    {
        public TViewModel Model { get; set; }

        object IFubuPageWithModel.GetModel()
        {
            return Model;
        }
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