using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Diagnostics.Core.Configuration.Policies
{
    public class DiagnosticsAttributeUrlPolicy : IUrlPolicy
    {
        public bool Matches(ActionCall call, IConfigurationObserver log)
        {
            if (!call.Method.HasAttribute<FubuDiagnosticsUrlAttribute>())
            {
                return false;
            }

            log.RecordCallStatus(call, "Matched on {0}".ToFormat(GetType().Name));
            return true;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            var routeDefinition = call.ToRouteDefinition();
            var urlAttribute = call.Method.GetAttribute<FubuDiagnosticsUrlAttribute>();
            routeDefinition.Append(DiagnosticsUrls.ToRelativeUrl(urlAttribute.Url));
            return routeDefinition;
        }
    }
}