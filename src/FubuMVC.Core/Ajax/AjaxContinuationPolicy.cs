using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;


namespace FubuMVC.Core.Ajax
{
    public class AjaxContinuationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph
                .Behaviors
                .Where(IsAjaxContinuation)
                .Each(chain =>
                          {
                              if(chain.OfType<AjaxContinuationNode>().Any()) return;

                              // Apply json formatting and http model binding coming up, but strip out
                              chain.MakeAsymmetricJson();
                              chain.Output.ClearAll(); // get rid of the output node

                              var call = chain.LastCall(); // won't be null after our filter
                              graph.Observer.RecordCallStatus(call, "Adding {0} directly after action call".ToFormat(typeof(AjaxContinuationNode).Name));
                              chain.Calls.Last().AddAfter(new AjaxContinuationNode());
                          
                          
                              
                          
                          });
        }

        public static bool IsAjaxContinuation(BehaviorChain chain)
        {
            var outputType = chain.ActionOutputType();
            return outputType != null && outputType.CanBeCastTo<AjaxContinuation>();
        }
    }
}