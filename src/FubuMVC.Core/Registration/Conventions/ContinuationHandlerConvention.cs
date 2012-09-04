using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    public class ContinuationHandlerConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Where(IsRedirectable).Each(call => call.AddAfter(new ContinuationNode()));
        }

        public static bool IsRedirectable(ActionCall action)
        {
            var outputType = action.OutputType();
            if (outputType == null) return false;

            return outputType.CanBeCastTo<FubuContinuation>() || outputType.CanBeCastTo<IRedirectable>();
        }
    }
}