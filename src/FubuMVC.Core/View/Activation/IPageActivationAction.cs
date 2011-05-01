using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View.Activation
{
    public interface IPageActivationAction
    {
        void Activate(IServiceLocator services, IFubuPage page);
    }
}