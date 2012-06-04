using System;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class DiagnosticUrlPolicy : IUrlPolicy
    {
        public const string DIAGNOSTICS_URL_ROOT = "_fubu";

        public bool Matches(ActionCall call, IConfigurationObserver log)
        {
            return call.HasAttribute<FubuDiagnosticsAttribute>();
        }

        public IRouteDefinition Build(ActionCall call)
        {
            MethodInfo method = call.Method;
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