using System.Linq;
using System.Collections.Generic;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Conventions
{
    [Title("Modifies the RouteDefinition of a Route to reflect the Input Type properties")]
    public class MissingRouteInputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(x => !x.IsPartialOnly && x.Route != null && x.Route.Input == null && x.InputType() != null)
                .Each(chain =>
                {
                    chain.Route.ApplyInputType(chain.InputType());
                });
        }
    }
}