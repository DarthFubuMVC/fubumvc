using System;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class FubuPartialRequestUrlPolicy : IUrlPolicy
    {
        public bool Matches(ActionCall call, IConfigurationObserver log)
        {
            if (!call.Method.HasAttribute<FubuPartialAttribute>()) return false;

            if (log.IsRecording)
            {
                log.RecordCallStatus(call,
                                     "Action '{0}' has the [{1}] defined. This action is only callable via a partial request from another action and cannot be navigated-to or routed-to from the client browser directly."
                                         .ToFormat(call.Method.Name, typeof (FubuPartialAttribute).Name));
            }

            return true;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            return new NulloRouteDefinition();
        }

        public void LogPolicyDecision(ActionCall call)
        {
            
        }
    }
}