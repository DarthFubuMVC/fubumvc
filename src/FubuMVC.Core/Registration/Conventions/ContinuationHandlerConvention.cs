using System;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class ContinuationHandlerConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Where(x => x.OutputType().CanBeCastTo<FubuContinuation>()).Each(call =>
            {
                call.InsertDirectlyAfter(new ContinuationNode());
            });
        }
    }
}