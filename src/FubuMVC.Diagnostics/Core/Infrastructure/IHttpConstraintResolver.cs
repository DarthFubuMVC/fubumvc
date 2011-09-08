using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    public interface IHttpConstraintResolver
    {
        string Resolve(BehaviorChain chain);
    }
}