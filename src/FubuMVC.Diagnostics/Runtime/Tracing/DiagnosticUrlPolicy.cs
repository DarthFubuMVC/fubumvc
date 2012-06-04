using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Diagnostics.Runtime.Tracing
{
    [MarkedForTermination("Not sure this is used")]
    public class DiagnosticUrlPolicy : IUrlPolicy
    {
        public const string DIAGNOSTICS_URL_ROOT = "_fubu";

        public bool Matches(ActionCall call, IConfigurationObserver log)
        {
            return call.HasAttribute<FubuDiagnosticsAttribute>();
        }

        public IRouteDefinition Build(ActionCall call)
        {
            var method = call.Method;
            var definition = call.ToRouteDefinition();
            definition.Append(DIAGNOSTICS_URL_ROOT + "/" + UrlFor(method));

            return definition;
        }

        public static string UrlFor(MethodInfo method)
        {
            return method.Name.ToLower();
        }
    }
}