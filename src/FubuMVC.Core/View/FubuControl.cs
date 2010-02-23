using System;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.Util;
using FubuMVC.Core.View.WebForms;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View
{
    public class FubuControl<TViewModel> : UserControl, IFubuPage<TViewModel>, INeedToKnowAboutParentPage where TViewModel : class
    {
        private readonly Cache<Type, object> _services;

        public void SetModel(IFubuRequest request)
        {
            Model = request.Get<TViewModel>();
        }

        public void SetModel(TViewModel model)
        {
            Model = model;
        }

        public TViewModel Model { get; private set; }

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

        public IUrlRegistry Urls
        {
            get { return Get<IUrlRegistry>(); }
        }

        string IFubuPage.ElementPrefix { get; set; }

        Page INeedToKnowAboutParentPage.ParentPage { get; set; }
    }
}