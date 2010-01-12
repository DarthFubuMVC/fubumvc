using FubuMVC.Core.Runtime;

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
}