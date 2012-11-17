using FubuCore.Descriptions;
using FubuMVC.Core.Registration;
using System.Collections.Generic;

namespace FubuMVC.Core.Security
{
    [Policy]
    [Title("Attaches the AuthorizationNode to a BehaviorChain if there are any authorization rules on the chain")]
    public class AttachAuthorizationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Each(x =>
            {
                if (!x.Authorization.HasRules()) return;
                x.Prepend(x.Authorization);
            });
        }
    }
}