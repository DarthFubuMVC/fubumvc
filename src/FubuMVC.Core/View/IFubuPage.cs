using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View
{
    public interface IFubuPage : IFubuView
    {
        IServiceLocator ServiceLocator { get; set; }
        T Get<T>();
        T GetNew<T>();
    }

    public interface IFubuPage<TViewModel> : IFubuPage, IFubuView<TViewModel> where TViewModel : class
    {
        
    }
}