using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime.Logging;
using FubuMVC.Diagnostics.Runtime;
using HtmlTags;

namespace FubuMVC.Diagnostics.Requests
{
    public class BehaviorNodeTraceTag : HtmlTag
    {
        public BehaviorNodeTraceTag(BehaviorNode node, RequestLog log) : base("li")
        {
            var description = Description.For(node);
            AddClass("node-trace").Data("node-id", node.UniqueId.ToString());

            var container = Add("div").AddClass("node-trace-container");
            container.Text(description.Title);

            var start = log.FindStep<BehaviorStart>(x => x.Correlation.Node == node);
            var finish = log.FindStep<BehaviorFinish>(x => x.Correlation.Node == node);

            if (start == null)
            {
                AddClass("gray");
                return;
            }

            // TODO -- What *should* happen here?
            if (finish == null)
            {
                return;
            }

            var exception = log.FindStep<ExceptionReport>(x => node.UniqueId.Equals(x.CorrelationId));
            if (exception != null || !finish.Log.Succeeded)
            {
                AddClass("exception");
            }

            var duration = finish.RequestTimeInMilliseconds - start.RequestTimeInMilliseconds;

            container.Add("span").Text(duration.ToString() + " ms").AddClass("node-trace-duration");
        }
    }
}