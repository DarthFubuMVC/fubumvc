using FubuCore;

namespace FubuMVC.Core.View.Activation
{
    /// <summary>
    ///   Implement this contract if you want to participate in the correct setup of
    ///   a View instance.
    /// </summary>
    public interface IPageActivationAction
    {
        void Activate(IServiceLocator services, IFubuPage page);
    }
}