using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Core.Security.Authentication
{
    [Description("Applies the built in Authentication to chains")]
    public class ApplyAuthenticationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<AuthenticationSettings>();
            var filter = settings.ExcludeChains.As<IChainFilter>();

            graph.Behaviors
                .OfType<RoutedChain>()
                .Where(x => !filter.Matches(x))
                .Each(x => x.Prepend(new AuthenticationFilterNode()));
        }
    }
}