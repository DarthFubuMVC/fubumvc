using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration
{
    [Obsolete("The usage is awkward and really doesn't provide much value over a custom IConfigurationAction")]
    public interface IRouteVisitor
    {
        void VisitRoute(IRouteDefinition route, BehaviorChain chain);
    }
}