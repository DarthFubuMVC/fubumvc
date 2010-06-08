using System.Collections.Generic;
using System.Linq;
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
            return _actionSources.SelectMany(actionSource => actionSource.FindActions(types));
        }

        //TODO -- Really similar to BehaviorMatcher. Can we reuse?
        private void registerBehavior(ActionCall call)
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(call);
            _graph.AddChain(chain);
        }
    }
}