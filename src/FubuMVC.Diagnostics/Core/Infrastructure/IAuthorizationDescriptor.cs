using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    public interface IAuthorizationDescriptor
    {
        IEndPointAuthorizor AuthorizorFor(BehaviorChain chain);
    }

    public class AuthorizationDescriptor : IAuthorizationDescriptor
    {
        private readonly IServiceLocator _locator;

        public AuthorizationDescriptor(IServiceLocator locator)
        {
            _locator = locator;
        }

        public IEndPointAuthorizor AuthorizorFor(BehaviorChain chain)
        {
            // TODO -- remove CSL usage here. EndpointAuthorizor is configured against the chain's id so this is needed for now
            return _locator.GetInstance<IEndPointAuthorizor>(chain.UniqueId.ToString());
        }
    }
}