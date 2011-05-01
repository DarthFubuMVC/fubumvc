using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View
{
    public interface IFubuPageWithModel : IFubuPage
    {
        void SetModel(IFubuRequest request);
        void SetModel(object model);
    }

    public interface IFubuPage
    {
        string ElementPrefix { get; set; }
        IServiceLocator ServiceLocator { get; set; }
        T Get<T>();
        T GetNew<T>();
        IUrlRegistry Urls { get; }
    }

    public interface IFubuPage<out TViewModel> : IFubuPage, IFubuPageWithModel where TViewModel : class
    {
        TViewModel Model { get; }
    }
}