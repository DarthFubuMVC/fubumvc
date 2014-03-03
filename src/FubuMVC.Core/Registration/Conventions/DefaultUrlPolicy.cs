using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.PathBased;

namespace FubuMVC.Core.Registration.Conventions
{
    [Description("Default Url Policy")]
    public class DefaultUrlPolicy : IUrlPolicy
    {
        public bool Matches(ActionCall call)
        {
            return true;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            var className = call.HandlerType.Name.ToLower()
                .Replace("endpoints", "")
                .Replace("endpoint", "")
                
                .Replace("controller", "");

            RouteDefinition route = null;
            if (RouteDefinition.VERBS.Any(x => x.EqualsIgnoreCase(call.Method.Name)))
            {
                route = new RouteDefinition(className);
                route.AddHttpMethodConstraint(call.Method.Name.ToUpper());
            }
            else
            {
                route = new RouteDefinition("{0}/{1}".ToFormat(className, call.Method.Name.ToLowerInvariant()));
            }

            if (call.InputType() != null)
            {
                if (call.InputType().CanBeCastTo<ResourcePath>())
                {
                    addResourcePathInputs(route);
                }
                else
                {
                    addBasicRouteInputs(call, route);
                }
            }

            return route;
        }

        private static void addBasicRouteInputs(ActionCall call, RouteDefinition route)
        {
            call.InputType()
                .GetProperties()
                .Where(x => x.HasAttribute<RouteInputAttribute>())
                .Each(prop => route.Append("{" + prop.Name + "}"));

            route.ApplyInputType(call.InputType());
        }

        private static void addResourcePathInputs(RouteDefinition route)
        {
            route.Append(ResourcePath.UrlSuffix);


            route.RegisterRouteCustomization(r => {
                r.Defaults.Add("Part0", null);
                r.Defaults.Add("Part1", null);
                r.Defaults.Add("Part2", null);
                r.Defaults.Add("Part3", null);
                r.Defaults.Add("Part4", null);
                r.Defaults.Add("Part5", null);
                r.Defaults.Add("Part6", null);
                r.Defaults.Add("Part7", null);
                r.Defaults.Add("Part8", null);
                r.Defaults.Add("Part9", null);
            });
        }
    }
}