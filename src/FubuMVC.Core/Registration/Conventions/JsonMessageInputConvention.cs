using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    public class JsonMessageInputConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(x => x.ResourceType().CanBeCastTo<JsonMessage>() || x.InputType().CanBeCastTo<JsonMessage>())
                .Each(x => x.MakeAsymmetricJson());
        }
    }
}