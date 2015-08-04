using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Security.Authentication
{
    [Description("Applies the built in Authentication to chains")]
    public class ApplyAuthenticationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<AuthenticationSettings>();

            graph.Chains.OfType<RoutedChain>()
                .Where(x => !settings.ShouldBeExcluded(x))
                .Each(x => x.Prepend(new AuthenticationFilterNode()));
        }

        public static bool IsMarkedAsNotAuthenticated(RoutedChain chain)
        {
            return chain.Calls.Any(ActionIsExempted);
        }

        public static bool ActionIsExempted(ActionCall call)
        {
            if (call.HasAttribute<NotAuthenticatedAttribute>()) return true;

            if (IsPassThrough(call)) return true;

            if (call.InputType() != null && call.InputType().HasAttribute<NotAuthenticatedAttribute>())
            {
                return true;
            }

            return false;
        }

        public static bool IsPassThrough(ActionCall call)
        {
            if (call.HasAttribute<PassThroughAuthenticationAttribute>()) return true;

            if (call.InputType() != null && call.InputType().HasAttribute<PassThroughAuthenticationAttribute>())
            {
                return true;
            }

            return false;
        }
    }
}