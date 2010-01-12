using System;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View
{
    public class FubuPage<TViewModel> : FubuPage, IFubuPage<TViewModel> where TViewModel : class
    {
        public void SetModel(IFubuRequest request)
        {
            Model = request.Get<TViewModel>();
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
    }
}