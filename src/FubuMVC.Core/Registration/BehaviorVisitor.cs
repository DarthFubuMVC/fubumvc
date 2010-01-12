using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Registration
{
    public class BehaviorVisitor : IBehaviorVisitor, IConfigurationAction
    {
        private readonly CompositeAction<BehaviorChain> _actions = new CompositeAction<BehaviorChain>();
        private readonly CompositePredicate<BehaviorChain> _filters = new CompositePredicate<BehaviorChain>();

        public CompositeAction<BehaviorChain> Actions { get { return _actions; } set { } }
        public CompositePredicate<BehaviorChain> Filters { get { return _filters; } set { } }

        public void VisitBehavior(BehaviorChain chain)
        {
            if (_filters.MatchesAll(chain))
            {
                _actions.Do(chain);
            }
        }

        void IConfigurationAction.Configure(BehaviorGraph graph)
        {
            graph.VisitBehaviors(this);
        }
    }
}