using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Core.Security.Authentication
{
    public class ApplyPassThroughAuthenticationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<AuthenticationSettings>();
            var filter = settings.PassThroughChains.As<IChainFilter>();

            graph.Behaviors
                .OfType<RoutedChain>()
                .Where(filter.Matches)
                .Each(x => x.Prepend(ActionFilter.For<PassThroughAuthenticationFilter>(a => a.Filter())));
        }
    }
}