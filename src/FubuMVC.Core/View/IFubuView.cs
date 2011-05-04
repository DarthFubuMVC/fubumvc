using System;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View
{
    public interface IFubuPage
    {
        string ElementPrefix { get; set; }
        IServiceLocator ServiceLocator { get; set; }
        IUrlRegistry Urls { get; }
        T Get<T>();
        T GetNew<T>();
    }

    public interface IFubuPageWithModel : IFubuPage
    {
        object GetModel();
    }

    public interface IFubuPage<TViewModel> : IFubuPageWithModel where TViewModel : class
    {
        TViewModel Model { get; set; }
    }

    public class SimpleFubuPage<TViewModel> : IFubuPage<TViewModel> where TViewModel : class
    {
        public SimpleFubuPage()
        {
            _services.OnMissing = type => ServiceLocator.GetInstance(type);
        }

        public string ElementPrefix { get; set; }

        public IServiceLocator ServiceLocator { get; set; }

        public IUrlRegistry Urls
        {
            get { return ServiceLocator.GetInstance<IUrlRegistry>(); }
        }

        private readonly Cache<Type, object> _services = new Cache<Type,object>();
        public T Get<T>()
        {
            return (T)_services[typeof(T)];
        }

        public T GetNew<T>()
        {
            return ServiceLocator.GetInstance<T>();
        }

        public object GetModel()
        {
            return Model;
        }

        public void SetModel(IFubuRequest request)
        {
            Model = request.Get<TViewModel>();
        }

        public TViewModel Model { get; set; }
    }
}