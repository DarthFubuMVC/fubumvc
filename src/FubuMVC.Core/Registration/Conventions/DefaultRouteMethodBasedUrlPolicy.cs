using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DefaultRouteMethodBasedUrlPolicy : IUrlPolicy
    {
        private readonly MethodInfo _method;

        public DefaultRouteMethodBasedUrlPolicy(MethodInfo method)
        {
            _method = method;
        }

        public bool Matches(ActionCall call)
        {
            return call.Method == _method;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            return call.ToRouteDefinition();
        }
    }
}