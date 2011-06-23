using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.Attachment
{
    public class ViewAttacherConvention : IViewBagConvention
    {
        private readonly List<IViewsForActionFilter> _filters = new List<IViewsForActionFilter>();

        public IEnumerable<IViewsForActionFilter> Filters
        {
            get { return _filters; }
        }

        public void AddViewsForActionFilter(IViewsForActionFilter filter)
        {
            _filters.Add(filter);
        }

        public void Configure(ViewBag bag, BehaviorGraph graph)
        {
            graph.Behaviors
                .Select(x => x.FirstCall())
                .Where(x => x != null)
                .Each(a => AttemptToAttachViewToAction(bag, a, graph.Observer));
        }

        public void AttemptToAttachViewToAction(ViewBag bag, ActionCall call, IConfigurationObserver observer)
        {
            foreach (var filter in _filters)
            {
                var viewTokens = filter.Apply(call, bag);
                var count = viewTokens.Count();

                observer.RecordCallStatus(call, "View filter '{0}' found {1} view token{2}".ToFormat(
                    filter.GetType().Name, count, (count != 1) ? "s" : "" ));

                if( count > 0 )
                {
                    viewTokens.Each(t =>
                        observer.RecordCallStatus(call, "Found view token: {0}".ToFormat(t)));
                }

                // if the filter returned more than one, consider it "failed", ignore it, and move on to the next
                if (count != 1) continue;

                var token = viewTokens.First();
                observer.RecordCallStatus(call, "Selected view token: {0}".ToFormat(token));
                call.AddToEnd(token.ToBehavioralNode());
                break;
            }
        }
    }
}