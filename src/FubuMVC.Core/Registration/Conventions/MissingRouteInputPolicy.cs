using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.Conventions
{
    public class MissingRouteInputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(x => !x.IsPartialOnly && x.Route.Input == null && x.InputType() != null)
                .Each(chain =>
                {
                    chain.Route.ApplyInputType(chain.InputType());
                });
        }
    }
}