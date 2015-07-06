using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Json
{
    public class ApplyJsonBindingPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<JsonBindingSettings>();
            var filter = settings.Include.As<IChainFilter>();

            graph
                .Behaviors
                .Where(filter.Matches)
                .Each(chain => chain.Output.Add(typeof(NewtonSoftBindingReader<>)));
        }
    }
}