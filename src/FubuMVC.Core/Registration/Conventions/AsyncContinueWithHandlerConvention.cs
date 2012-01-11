using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class AsyncContinueWithHandlerConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Where(x => x.IsAsync).Each(call =>
            {
                call.WrapWith<AsyncInterceptExceptionBehavior>();
                call.AddAfter(call.Method.ReturnType == typeof(Task)
                                  ? new AsyncContinueWithNode()
                                  : new AsyncContinueWithNode(call.OutputType()));
                graph.Observer.RecordCallStatus(call, "Adding AsyncContinueWithNode directly after action call");
            });
        }
    }
}