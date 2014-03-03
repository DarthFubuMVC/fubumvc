using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Diagnostics
{
    [Title("Register the _about endpoint")]
    public class RegisterAbout : IChainSource
    {
        public IEnumerable<BehaviorChain> BuildChains(SettingsCollection settings)
        {
            yield return addAboutEndpoint();
            yield return addReloadedEndpoint();
        }

        private static BehaviorChain addAboutEndpoint()
        {
            var action = ActionCall.For<AboutEndpoint>(x => x.get__about());
            var chain = new BehaviorChain();
            chain.AddToEnd(action);
            chain.Route = new RouteDefinition("_about");

            return chain;
        }

        private static BehaviorChain addReloadedEndpoint()
        {
            var action = ActionCall.For<AboutEndpoint>(x => x.get__loaded());
            var chain = new BehaviorChain();
            chain.AddToEnd(action);
            chain.Route = new RouteDefinition("_loaded");

            return chain;
        }
    }
}