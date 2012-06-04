using System;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuMVC.Diagnostics.Runtime;

namespace FubuMVC.Diagnostics.Core.Configuration.Policies
{
    public class DiagnosticsHandlerUrlPolicy : HandlersUrlPolicy
    {
        public DiagnosticsHandlerUrlPolicy(params Type[] markerTypes)
            : base(markerTypes)
        {
        }

        public override bool Matches(ActionCall call, IConfigurationObserver log)
        {
            if(!call.IsDiagnosticsHandler())
            {
                return false;
            }

            return base.Matches(call, log);
        }

        protected override void visit(IRouteDefinition routeDefinition)
        {
            routeDefinition.Append(DiagnosticsUrls.ROOT);
        }
    }
}