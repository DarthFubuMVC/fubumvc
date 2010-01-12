using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    public interface IRouteInputPolicy
    {
        bool Matches(Type inputType);
        void AlterRoute(IRouteDefinition route, ActionCall call);
    }
}