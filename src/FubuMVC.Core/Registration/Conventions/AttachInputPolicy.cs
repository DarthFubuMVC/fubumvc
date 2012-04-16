using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Registration.Conventions
{
    public class AttachInputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.HasReaders()).Each(x => x.Prepend(x.Input));
        }
    }
}