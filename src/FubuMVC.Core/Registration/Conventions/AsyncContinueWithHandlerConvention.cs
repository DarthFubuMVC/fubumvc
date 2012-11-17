using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    [Description("Adds the necessary behaviors to a chain for asynchronous Behavior Chain's that contain an action that returns a Task")]
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
            });
        }
    }
}