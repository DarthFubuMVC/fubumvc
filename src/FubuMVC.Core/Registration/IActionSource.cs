using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public interface IActionSource
    {
        IEnumerable<ActionCall> FindActions(TypePool types);
    }

    public class ActionSourceMatcher
    {
        private BehaviorGraph _graph;
        private readonly List<IActionSource> _actionSources = new List<IActionSource>();
        public void BuildBehaviors(TypePool pool, BehaviorGraph graph)
        {
            _graph = graph;
            buildActions(pool).Each(registerBehavior);
            _graph = null;
        }

        public void AddSource(IActionSource source)
        {
            _actionSources.Add(source);
        }

        private IEnumerable<ActionCall> buildActions(TypePool types)
        {
            foreach (var actionSource in _actionSources)
            {
                foreach (var action in actionSource.FindActions(types))
                {
                    yield return action;
                }
            }
        }

        private void registerBehavior(ActionCall call)
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(call);
            _graph.AddChain(chain);
        }
    }
}