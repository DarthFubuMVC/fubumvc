using System;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Registration.Conventions
{
    public class ActionCallModification : IConfigurationAction
    {
        private readonly CompositeFilter<ActionCall> _filters = new CompositeFilter<ActionCall>();
        private readonly Action<ActionCall> _modification;

        public ActionCallModification(Action<ActionCall> modification)
        {
            _modification = modification;
        }

        public CompositeFilter<ActionCall> Filters { get { return _filters; } set { } }

        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Where(_filters.Matches).Each(_modification);
        }
    }
}