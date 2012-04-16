using FubuMVC.Core.Registration.Nodes;
using GenericEnumerableExtensions = System.Collections.Generic.GenericEnumerableExtensions;

namespace FubuMVC.Core.Registration.Conventions
{
    public class AttachInputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            GenericEnumerableExtensions.Each<BehaviorChain>(graph.Behaviors.Where(x => x.HasReaders()), x => x.Prepend(x.Input));
        }
    }
}