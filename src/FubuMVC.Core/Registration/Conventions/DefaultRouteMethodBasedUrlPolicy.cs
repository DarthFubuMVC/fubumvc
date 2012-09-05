using System;
using System.Reflection;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DefaultRouteMethodBasedUrlPolicy : IUrlPolicy, DescribesItself
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

        void DescribesItself.Describe(Description description)
        {
            description.Title =
                "'Home' route should be the endpoint that calls {0}.{1}()".ToFormat(_method.DeclaringType.Name,
                                                                                    _method.Name);
        }
    }
}