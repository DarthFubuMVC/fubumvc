using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.Attachment
{
    public class ViewAttacher : IConfigurationAction
    {
        private readonly List<IViewFacility> _facilities = new List<IViewFacility>();
        private readonly List<IViewsForActionFilter> _filters = new List<IViewsForActionFilter>();
        private readonly TypePool _types;

        public ViewAttacher(TypePool types)
        {
            _types = types;
        }

        public void Configure(BehaviorGraph graph)
        {
            _types.ShouldScanAssemblies = true;
            var views = new List<IViewToken>();

            foreach (var facility in _facilities)
            {
                views.AddRange(facility.FindViews(_types, graph));
            }

            var bag = new ViewBag(views);

            graph.Behaviors
                .Select(x => x.FirstCall())
                .Where(x => x != null)
                .Each(a => AttemptToAttachViewToAction(bag, a, graph.Observer));
        }

        public List<IViewFacility> Facilities
        {
            get { return _facilities; }
        }

        public IEnumerable<IViewsForActionFilter> Filters
        {
            get { return _filters; }
        }

        public TypePool Types
        {
            get { return _types; }
        }

        public void AddFacility(IViewFacility facility)
        {
            var typeOfFacility = facility.GetType();
            if(_facilities.Any(f => f.GetType() == typeOfFacility)) return;
            _facilities.Add(facility);
        }

        public void AddViewsForActionFilter(IViewsForActionFilter filter)
        {
            _filters.Add(filter);
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