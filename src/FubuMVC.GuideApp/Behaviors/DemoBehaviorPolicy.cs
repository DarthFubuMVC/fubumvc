using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.GuideApp.Behaviors
{
    public class DemoBehaviorPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(c => c.FirstCall().Method.Name == "Home")
                .Each(c => c.Prepend(new Wrapper(typeof (DemoBehaviorForSelectActions))));
        }
    }
}