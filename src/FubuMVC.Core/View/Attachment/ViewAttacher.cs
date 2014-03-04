using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.View.Attachment
{

    public class ViewAttachmentWorker
    {
        private readonly ViewBag _views;
        private readonly ViewAttachmentPolicy _policy;

        public ViewAttachmentWorker(ViewBag views, ViewAttachmentPolicy policy)
        {
            _views = views;
            _policy = policy;
        }

        public void Configure(IEnumerable<BehaviorChain> chains)
        {
            if (!_views.Views.Any()) return;

            FindLastActions(chains).Each(attachToAction);
        }

        private void attachToAction(ActionCall action)
        {
            _policy.Profiles(_views).Each(x => Attach(x.Profile, x.Views, action));
        }

        public virtual void Attach(IViewProfile viewProfile, ViewBag bag, ActionCall action)
        {
            // No duplicate views!
            var outputNode = action.ParentChain().Output;
            if (outputNode.HasView(viewProfile.Condition)) return;


            foreach (var filter in _policy.Filters())
            {
                var viewTokens = filter.Apply(action, bag);
                var count = viewTokens.Count();

                if (count != 1) continue;

                var token = viewTokens.Single().Resolve();
                outputNode.AddView(token, viewProfile.Condition);

                break;
            }
        }



        public static IEnumerable<ActionCall> FindLastActions(IEnumerable<BehaviorChain> chains)
        {
            foreach (var chain in chains)
            {
                var last = chain.Calls.LastOrDefault();
                if (last != null && last.HasOutput)
                {
                    yield return last;
                }
            }
        }
    }


    [Policy]
    [Description("View Attachment")]
    public class ViewAttacher : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var views = graph.Settings.Get<ViewEngines>().BuildViewBag(graph.Settings);

            Configure(graph, views);
        }

        public void Configure(BehaviorGraph graph, ViewBag views)
        {
            var policy = graph.Settings.Get<ViewAttachmentPolicy>();

            var worker = new ViewAttachmentWorker(views, policy);

            worker.Configure(graph.Behaviors);
        }


    }
}