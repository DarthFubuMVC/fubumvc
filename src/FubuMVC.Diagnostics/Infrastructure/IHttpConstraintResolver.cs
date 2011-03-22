using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Infrastructure
{
    public interface IHttpConstraintResolver
    {
        string Resolve(BehaviorChain chain);
    }
}