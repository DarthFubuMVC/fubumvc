using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    [ConfigurationType(ConfigurationType.Discovery)]
    public class ActionSourceRunner : IConfigurationAction, DescribesItself
    {
        private readonly IActionSource _source;

        public ActionSourceRunner(IActionSource source)
        {
            _source = source;
        }

        public void Configure(BehaviorGraph graph)
        {
            var actions = _source.FindActions(graph.ApplicationAssembly);

            var existing = graph.Actions().ToList();

            actions.Where(x => !existing.Contains(x)).Each(call => {
                var chain = new BehaviorChain();
                chain.AddToEnd(call);
                graph.AddChain(chain);
            });
        }

        public void Describe(Description description)
        {
            description.Title = "ActionSource";
            description.AddChild("Source", _source);
        }
    }
}