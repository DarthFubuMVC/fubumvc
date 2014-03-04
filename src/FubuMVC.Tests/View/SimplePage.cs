using System;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;

namespace FubuMVC.Tests.View
{
    public class SimpleFubuPage<TViewModel> : IFubuPage<TViewModel> where TViewModel : class
    {
        private readonly Cache<Type, object> _services = new Cache<Type, object>();

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

        public T Get<T>()
        {
            return (T)_services[typeof(T)];
        }

        public T GetNew<T>()
        {
            return ServiceLocator.GetInstance<T>();
        }

        public void Write(object content)
        {
            ServiceLocator.GetInstance<IOutputWriter>().WriteHtml(content);
        }

        public object GetModel()
        {
            return Model;
        }

        public TViewModel Model { get; set; }

        public void SetModel(IFubuRequest request)
        {
            Model = request.Get<TViewModel>();
        }
    }
}