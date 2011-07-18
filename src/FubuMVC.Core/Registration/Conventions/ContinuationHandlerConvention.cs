using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using System;

namespace FubuMVC.Core.Registration.Conventions
{
    public class ContinuationHandlerConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Where(x => x.OutputType().CanBeCastTo<FubuContinuation>()).Each(call =>
            {
                call.AddAfter(new ContinuationNode());
                graph.Observer.RecordCallStatus(call, "Adding ContinuationNode directly after action call");
            });
        }
    }

    public class RedirectableHandlerConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Where(x => x.OutputType().CanBeCastTo<Redirectable>()).Each(call => {
                var outputType = call.OutputType();
                if (outputType.IsGenericType) {
                    var genericParameter = outputType.GetGenericArguments()[0];
                    var node = typeof(RedirectableNode<>).MakeGenericType(genericParameter);
                    call.AddAfter((BehaviorNode)Activator.CreateInstance(node));
                    graph.Observer.RecordCallStatus(call, "Adding redirectableNode directly after action call");
                }
            });
        }
    }
}