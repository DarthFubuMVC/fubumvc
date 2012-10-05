using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.Attachment
{
    [Policy]
    public class ViewAttacher : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Settings.Get<ViewEngines>().UseGraph(graph);
            var policy = graph.Settings.Get<ViewAttachmentPolicy>();

            FindLastActions(graph).Where(x => x.OutputType() != null).Each(action => {
                policy.Profiles(graph).Each(x => {
                    Attach(policy, x.Profile, x.Views, action);
                });
            });
        }

        public virtual void Attach(ViewAttachmentPolicy policy, IViewProfile viewProfile, ViewBag bag, ActionCall action)
        {
            // No duplicate views!
            var outputNode = action.ParentChain().Output;
            if (outputNode.HasView(viewProfile.ConditionType)) return;

            var log = new ViewAttachmentLog(viewProfile);
            action.Trace(log);

            foreach (var filter in policy.Filters())
            {
                var viewTokens = filter.Apply(action, bag);
                var count = viewTokens.Count();

                if (count > 0)
                {
                    log.FoundViews(filter, viewTokens.Select(x => x.Resolve()));
                }

                if (count != 1) continue;

                var token = viewTokens.Single().Resolve();
                outputNode.AddView(token, viewProfile.ConditionType);

                break;
            }
        }



        public static IEnumerable<ActionCall> FindLastActions(BehaviorGraph graph)
        {
            foreach (var chain in graph.Behaviors)
            {
                var last = chain.Calls.LastOrDefault();
                if (last != null)
                {
                    yield return last;
                }
            }
        }


    }
}