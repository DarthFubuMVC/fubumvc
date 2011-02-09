namespace FubuMVC.Core.View
{
    public interface IViewActivator
    {
        void Activate<T>(IFubuPage<T> page) where T : class;
        void Activate(IFubuPage page);
    }
}