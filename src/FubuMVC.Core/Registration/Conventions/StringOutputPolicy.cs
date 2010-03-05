using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class StringOutputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => !x.HasOutputBehavior() && x.ActionOutputType() == typeof (string)).Each(
                x => { x.AddToEnd(new RenderTextNode<string>()); });
        }
    }
}