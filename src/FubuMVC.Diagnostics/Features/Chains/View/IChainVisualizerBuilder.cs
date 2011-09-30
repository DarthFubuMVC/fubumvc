using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Infrastructure;
using HtmlTags;

namespace FubuMVC.Diagnostics.Features.Chains.View
{
    public interface IChainVisualizerBuilder
    {
        ChainModel VisualizerFor(Guid uniqueId);
    }

    public class ChainVisualizerBuilder : IChainVisualizerBuilder
    {
        private readonly BehaviorGraph _graph;
        private readonly IHttpConstraintResolver _constraintResolver;

        public ChainVisualizerBuilder(BehaviorGraph graph, IHttpConstraintResolver constraintResolver)
        {
            _graph = graph;
            _constraintResolver = constraintResolver;
        }

        public ChainModel VisualizerFor(Guid uniqueId)
        {
            var chain = _graph
                .Behaviors
                .SingleOrDefault(c => c.UniqueId == uniqueId);

            if(chain == null)
            {
                return null;
            }

            return new ChainModel
                       {
                           Chain = chain,
                           Constraints = _constraintResolver.Resolve(chain),
                           Behaviors = chain.Select(x =>
                                                        {
                                                            var behavior = new BehaviorModel
                                                                               {
                                                                                   Id = x.UniqueId,
                                                                                   DisplayType = x.GetType().PrettyPrint(),
                                                                                   BehaviorType = x.ToString(),
                                                                                   BehaviorLabel = BuildLabel(x)
                                                                               };
                                                            var call = x as ActionCall;
                                                            if (call != null)
                                                            {
                                                                behavior.Logs = _graph.Observer.GetLog(call);
                                                            }

                                                            return behavior;
                                                        })
                       };
        }

        //this is hideous but wanted to show an idea.
        public HtmlTag BuildLabel(BehaviorNode behaviorNode)
        {
            var span = new HtmlTag("span").AddClass("label");
            var pp = behaviorNode.GetType().PrettyPrint();
            span.Text("behavior");
            
            if (pp.StartsWith("WebForm"))
            {
                span.Text("View");
                span.AddClass("notice");
            }
            else if (pp.StartsWith("Call"))
            {
                span.AddClass("warning");
                span.Text("continuation");
            }
            else if (!behaviorNode.GetType().Name.StartsWith("Fubu"))
            {
                span.Text("fubu");
            }
            
            return span;
        }
    }

 
}