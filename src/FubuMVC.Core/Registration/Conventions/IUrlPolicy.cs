using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    public interface IUrlPolicy
    {
        bool Matches(ActionCall call);
        IRouteDefinition Build(ActionCall call);
    }
}