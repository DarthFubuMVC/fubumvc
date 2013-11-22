using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;
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
            return call.Method.Matches(_method);
        }

        public IRouteDefinition Build(ActionCall call)
        {
            // I hate this, but it works.
            var routeDefinition = call.ToRouteDefinition().As<RouteDefinition>();
            if (MethodToUrlBuilder.Matches(call.Method.Name))
            {
                MethodToUrlBuilder.AddHttpConstraints(routeDefinition, call.Method.Name, txt => { });
            }


            return routeDefinition;
        }

        void DescribesItself.Describe(Description description)
        {
            description.Title =
                "'Home' route should be the endpoint that calls {0}.{1}()".ToFormat(_method.DeclaringType.Name,
                                                                                    _method.Name);
        }
    }
}