using System.Collections.Generic;

namespace FubuMVC.Core.Registration.Nodes
{
    public static class BehaviorExtensions
    {
        public static void Configure(this List<IConfigurationAction> actions, BehaviorGraph graph)
        {
            actions.Each(x => x.Configure(graph));
        }
    }
}