using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;


namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    // TODO -- what is this?  Why do we need it?
    [MarkedForTermination("Think we can make this stuff go away w/ the new Description model in FubuCore")]
    public interface IAuthorizationDescriptor
    {
        IEndPointAuthorizor AuthorizorFor(BehaviorChain chain);
    }

    public class AuthorizationDescriptor : IAuthorizationDescriptor
    {
        private readonly IBehaviorFactory _factory;

        public AuthorizationDescriptor(IBehaviorFactory factory)
        {
            _factory = factory;
        }

        public IEndPointAuthorizor AuthorizorFor(BehaviorChain chain)
        {
            return _factory.AuthorizorFor(chain.UniqueId);
        }
    }
}