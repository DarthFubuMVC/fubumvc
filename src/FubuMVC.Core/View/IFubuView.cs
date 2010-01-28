using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View
{
    public interface IFubuView
    {
        
    }

    public interface IFubuViewWithModel : IFubuView
    {
        void SetModel(IFubuRequest request);
    }

    public interface IFubuView<VIEWMODEL> : IFubuViewWithModel
        where VIEWMODEL : class
    {
        VIEWMODEL Model { get; }
    }

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