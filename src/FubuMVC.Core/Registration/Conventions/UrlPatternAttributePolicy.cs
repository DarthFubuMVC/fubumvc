using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Registration.Conventions
{
    public class UrlPatternAttributePolicy : IUrlPolicy
    {
        public bool Matches(ActionCall call)
        {
            return call.Method.HasCustomAttribute<UrlPatternAttribute>();
        }

        public IRouteDefinition Build(ActionCall call)
        {
            var pattern = call.Method.GetAttribute<UrlPatternAttribute>().Pattern;
            
            return call.HasInput 
                       ? RouteBuilder.Build(call.InputType(), pattern) 
                       : new RouteDefinition(pattern);
        }
    }
}