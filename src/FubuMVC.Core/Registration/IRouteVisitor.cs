using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration
{
    public interface IRouteVisitor
    {
        void VisitRoute(IRouteDefinition route, BehaviorChain chain);
    }
}