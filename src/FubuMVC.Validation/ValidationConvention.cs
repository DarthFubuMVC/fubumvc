using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Validation
{
    public class ValidationConvention : IConfigurationAction
    {
        private readonly Func<ActionCall, bool> _predicate;

        public ValidationConvention(Func<ActionCall, bool> predicate)
        {
            _predicate = predicate;
        }


        public void Configure(BehaviorGraph graph)
        {
            graph
                .Actions()
                .Where(call => _predicate(call))
                .Each(call =>
                          {
                              var log = graph.Observer;
                              if(log.IsRecording)
                              {
                                  log.RecordCallStatus(call, "Wrapping {0} with Validation Behavior".ToFormat(call));
                              }

                              var input = call.InputType();
                              var behaviorType = typeof (ValidationBehavior<>).MakeGenericType(input);
                              call.WrapWith(behaviorType);
                          });
        }
    }
}