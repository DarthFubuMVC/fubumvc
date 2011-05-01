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
}