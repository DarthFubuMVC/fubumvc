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
            graph.Actions()
                .Where(x => x.InputType().CanBeCastTo<JsonMessage>())
                .ToList()
                .Each(x => x.ParentChain().MakeAsymmetricJson());
        }
    }
}