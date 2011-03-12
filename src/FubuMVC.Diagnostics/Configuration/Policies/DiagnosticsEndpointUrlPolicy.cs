using System;
using System.Text;
using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Diagnostics.Endpoints;

namespace FubuMVC.Diagnostics.Configuration.Policies
{
    public class DiagnosticsEndpointUrlPolicy : IUrlPolicy
    {
        public const string ENDPOINT = "Endpoint";

        public bool Matches(ActionCall call, IConfigurationObserver log)
        {
            if(!call.IsDiagnosticsEndpoint())
            {
                return false;
            }

            log.RecordCallStatus(call, "Matched on {0}".ToFormat(GetType().Name));
            return true;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            var routeDefinition = call.ToRouteDefinition();

            var strippedNamespace = call
                                        .HandlerType
                                        .Namespace
                                        .Replace(typeof (DiagnosticsEndpointMarker).Namespace + ".", string.Empty);
            routeDefinition.Append(DiagnosticsUrls.ROOT);
            if (strippedNamespace != call.HandlerType.Namespace)
            {
                if (!strippedNamespace.Contains("."))
                {
                    routeDefinition.Append(BreakUpCamelCaseWithHypen(strippedNamespace));
                }
                else
                {
                    var patternParts = strippedNamespace.Split(new[] { "." }, StringSplitOptions.None);
                    foreach (var patternPart in patternParts)
                    {
                        routeDefinition.Append(BreakUpCamelCaseWithHypen(patternPart.Trim()));
                    }
                }
            }

            routeDefinition.Append(BreakUpCamelCaseWithHypen(call.HandlerType.Name.Replace(ENDPOINT, string.Empty)));
            routeDefinition.ApplyRouteInputAttributes(call);
            return routeDefinition;
        }

        private static string BreakUpCamelCaseWithHypen(string input)
        {
            var routeBuilder = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                if (i != 0 && char.IsUpper(input[i]))
                {
                    routeBuilder.Append("-");
                }

                routeBuilder.Append(input[i]);
            }

            return routeBuilder
                        .ToString()
                        .ToLower();
        }
    }
}