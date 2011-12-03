using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class AsyncContinueWithHandlerConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Where(x => x.IsAsync).Each(call =>
            {
                call.AddAfter(new AsyncContinueWithNode(call.OutputType()));
                graph.Observer.RecordCallStatus(call, "Adding AsyncContinueWithNode directly after action call");
            });
        }
    }
}