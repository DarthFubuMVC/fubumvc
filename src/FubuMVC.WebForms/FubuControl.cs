using System;
using System.Web.UI;
using FubuCore.Util;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.WebForms
{
    public class FubuControl<TViewModel> : UserControl, IFubuPage<TViewModel>, INeedToKnowAboutParentPage where TViewModel : class
    {
        private readonly Cache<Type, object> _services;

        public TViewModel Model { get; set; }

        public FubuControl()
        {
            _services = new Cache<Type, object>
                            {
                                OnMissing = type => ServiceLocator.GetInstance(type)
                            };
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

        object IFubuPageWithModel.GetModel()
        {
            return Model;
        }

        public IUrlRegistry Urls
        {
            get { return Get<IUrlRegistry>(); }
        }

        string IFubuPage.ElementPrefix { get; set; }

        public Page ParentPage { get; set; }
    }
}