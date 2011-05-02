using System;
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
        public string ElementPrefix { get; set; }

        public IServiceLocator ServiceLocator { get; set; }

        public IUrlRegistry Urls
        {
            get { return ServiceLocator.GetInstance<IUrlRegistry>(); }
        }

        public T Get<T>()
        {
            return ServiceLocator.GetInstance<T>();
        }

        public T GetNew<T>()
        {
            return ServiceLocator.GetInstance<T>();
        }

        public object GetModel()
        {
            return Model;
        }

        public TViewModel Model { get; set; }
    }
}