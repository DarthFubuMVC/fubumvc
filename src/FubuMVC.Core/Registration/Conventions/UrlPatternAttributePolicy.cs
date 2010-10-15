using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class UrlPatternAttributePolicy : IUrlPolicy
    {
        public bool Matches(ActionCall call, IConfigurationObserver log)
        {
            var result = call.Method.HasAttribute<UrlPatternAttribute>();

            if( result && log.IsRecording )
            {
                log.RecordCallStatus(call, 
                    "Action '{0}' has the [{1}] defined. Using explicitly defined URL pattern."
                    .ToFormat(call.Method.Name, typeof(UrlPatternAttribute).Name));
            }

            return result;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            var pattern = call.Method.GetAttribute<UrlPatternAttribute>().Pattern;
            return call.BuildRouteForPattern(pattern);
   

        }
    }
}