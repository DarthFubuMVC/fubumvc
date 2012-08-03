using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DefaultRouteConventionBasedUrlPolicy : IUrlPolicy
    {
        private const string DefaultHomeControllerConvention = "HomeEndpoint";
        private const string DefaultHomeMethodConvention = "Index";

        public bool Matches(ActionCall call, IConfigurationObserver log)
        {
            return call.Method.DeclaringType.Name == DefaultHomeControllerConvention && 
                   call.Method.Name == DefaultHomeMethodConvention;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            return call.ToRouteDefinition();
        }
    }
}