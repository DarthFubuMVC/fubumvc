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
    // TODO -- look at making this little monster faster.
    [Description("The get_*, post_*, put_*, delete_* url convention")]
    public class MethodToUrlBuilder : IUrlPolicy
    {
        public bool Matches(ActionCall call)
        {
            return Matches(call.Method.Name);
        }

        public IRouteDefinition Build(ActionCall call)
        {
            var route = call.ToRouteDefinition();
            Alter(route, call);

            return route;
        }

        public static bool Matches(string methodName)
        {
            return methodName.Contains("_");
        }

        public static void Alter(IRouteDefinition route, ActionCall call)
        {
            var properties = call.HasInput
                ? new TypeDescriptorCache().GetPropertiesFor(call.InputType()).Keys
                : new string[0];

            Alter(route, call.Method.Name, properties);

            if (call.HasInput)
            {
                route.ApplyInputType(call.InputType());

                if (call.InputType().CanBeCastTo<ResourcePath>())
                {

                     ResourcePath.AddResourcePathInputs(route);
                }
            }
        }

        public static void Alter(IRouteDefinition route, string methodName, IEnumerable<string> properties)
        {
            var parts = AddHttpConstraints(route, methodName);

            for (var i = 0; i < parts.Count; i++)
            {
                var part = parts[i];
                if (properties.Contains(part))
                {
                    parts[i] = "{" + part + "}";
                }
                else
                {
                    parts[i] = part.ToLower();
                }
            }

            if (parts.Any())
            {
                route.Append(parts.Join("/").Replace('^', '_'));
            }
        }

        public static List<string> AddHttpConstraints(IRouteDefinition route, string methodName)
        {
            var parts = methodName.Replace("___", "-").Replace("__", "_^").Split('_').ToList();

            var method = parts.First().ToUpper();
            if (RouteDefinition.VERBS.Contains(method))
            {
                route.AddHttpMethodConstraint(method);
                parts.RemoveAt(0);
            }
            return parts;
        }
    }
}