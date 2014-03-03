using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    [Title("Method named HomeEndpoint.Index() is the default route if none is explicitly registered")]
    public class DefaultRouteConventionBasedUrlPolicy : IUrlPolicy
    {
        private const string DefaultHomeControllerConvention = "HomeEndpoint";
        private const string DefaultHomeMethodConvention = "Index";

        public bool Matches(ActionCall call)
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