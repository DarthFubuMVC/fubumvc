using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public class VisitBehaviorsAction : IConfigurationAction
    {
        private readonly Action<BehaviorVisitor> _configureAction;
        private readonly string _reasonToVisit;

        public VisitBehaviorsAction(Action<BehaviorVisitor> configureAction, string reasonToVisit)
        {
            _configureAction = configureAction;
            _reasonToVisit = reasonToVisit;
        }

        public void Configure(BehaviorGraph graph)
        {
            var visitor = new BehaviorVisitor(graph.Observer, _reasonToVisit);
            _configureAction(visitor);
            graph.VisitBehaviors(visitor);
        }
    }

    public class BehaviorVisitor : IBehaviorVisitor
    {
        private readonly IConfigurationObserver _observer;
        private readonly string _reasonToVisit;
        private readonly CompositeAction<BehaviorChain> _actions = new CompositeAction<BehaviorChain>();
        private readonly CompositePredicate<BehaviorChain> _filters = new CompositePredicate<BehaviorChain>();

        public CompositeAction<BehaviorChain> Actions { get { return _actions; } set { } }
        public CompositePredicate<BehaviorChain> Filters { get { return _filters; } set { } }

        public BehaviorVisitor(IConfigurationObserver observer, string reasonToVisit)
        {
            _observer = observer;
            _reasonToVisit = reasonToVisit;
        }

        public void VisitBehavior(BehaviorChain chain)
        {
            if (!_filters.MatchesAll(chain)) return;

            var matchesDescriptions = _filters.GetDescriptionOfMatches(chain).Join(", ");
            if( matchesDescriptions == string.Empty)
            {
                matchesDescriptions = "(no filters defined)";
            }

            chain.Calls.Each(call => _observer.RecordCallStatus(call,
                "Visiting: {0}. Matched on filters [{1}]".ToFormat
                    (_reasonToVisit, matchesDescriptions)));

            _actions.Do(chain);
        }
    }
}