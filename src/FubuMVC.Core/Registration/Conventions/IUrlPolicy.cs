using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Registration.Conventions
{
    public interface IUrlPolicy
    {
        bool Matches(ActionCall call, IConfigurationObserver log);
        IRouteDefinition Build(ActionCall call);
    }
}