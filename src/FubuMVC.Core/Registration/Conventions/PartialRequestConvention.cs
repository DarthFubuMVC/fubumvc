using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class PartialRequestConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var partialActions = graph.Behaviors.SelectMany(x => x.Calls).Where(x => x.HasAttribute<FubuPartialAttribute>());
            Action<ActionCall> logging = call => { };
                
                
            if (graph.Observer.IsRecording)
            {
                logging = call =>
                {
                    var message = "Action '{0}' has the [{1}] defined. This action is only callable via a partial request from another action and cannot be navigated-to or routed-to from the client browser directly."
                        .ToFormat(call.Method.Name, typeof (FubuPartialAttribute).Name);
                    graph.Observer.RecordCallStatus(call, message);
                };
            }

            partialActions.Each(call =>
            {
                call.ParentChain().IsPartialOnly = true;
                logging(call);
            });

        }
    }
}