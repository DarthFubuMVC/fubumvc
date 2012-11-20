using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FubuMVC.Core.Registration.Conventions
{
    [Description("Attaches the InputNode to a BehaviorChain if there are any readers registered")]
    public class AttachInputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.HasReaders()).Each(x => x.Prepend(x.Input));
        }
    }
}