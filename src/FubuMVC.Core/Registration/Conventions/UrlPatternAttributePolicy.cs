using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    [Title("Explicitly definined route via the [UrlPattern]")]
    public class UrlPatternAttributePolicy : IUrlPolicy
    {
        public bool Matches(ActionCall call)
        {
            return call.HasAttribute<UrlPatternAttribute>();
        }

        public IRouteDefinition Build(ActionCall call)
        {
            return call.Method.GetAttribute<UrlPatternAttribute>().BuildRoute(call.InputType());
        }
    }
}