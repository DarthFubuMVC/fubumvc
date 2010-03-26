using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class ActionCallModification : IConfigurationAction
    {
        private readonly CompositeFilter<ActionCall> _filters = new CompositeFilter<ActionCall>();
        private readonly Action<ActionCall> _modification;
        private readonly string _reasonForModification;

        public ActionCallModification(Action<ActionCall> modification, string reasonForModification)
        {
            _modification = modification;
            _reasonForModification = reasonForModification;
        }

        public CompositeFilter<ActionCall> Filters { get { return _filters; } set { } }

        public void Configure(BehaviorGraph graph)
        {
            graph.Actions()
                .Where(_filters.Matches)
                .Each(call=>
                {
                    var matchingFilter = _filters.GetDescriptionOfFirstMatchingInclude(call);

                    var log = "{0} [matched on filter '{1}']".ToFormat(_reasonForModification, matchingFilter);

                    graph.Observer.RecordCallStatus(call, log);
                    _modification(call);
                    
                });
        }
    }
}