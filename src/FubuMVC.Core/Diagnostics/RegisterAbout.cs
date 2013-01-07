using FubuCore.Descriptions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Diagnostics
{
    [Title("Register the _about endpoint")]
    public class RegisterAbout : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var action = ActionCall.For<AboutEndpoint>(x => x.get__about());
            var chain = new BehaviorChain();
            chain.AddToEnd(action);
            chain.Route = new RouteDefinition("_about");

            graph.AddChain(chain);
        }
    }
}