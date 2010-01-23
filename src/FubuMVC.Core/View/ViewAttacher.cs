using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View.WebForms;

namespace FubuMVC.Core.View
{
    public class ViewAttacher : IConfigurationAction
    {
        private readonly List<IViewFacility> _facilities = new List<IViewFacility>();
        private readonly List<IViewsForActionFilter> _filters = new List<IViewsForActionFilter>();
        private readonly TypePool _types;

        public ViewAttacher(TypePool types)
        {
            _types = types;

            AddFacility(new WebFormViewFacility());
        }

        public void Configure(BehaviorGraph graph)
        {
            var views = _facilities.SelectMany(x => x.FindViews(_types));
            var bag = new ViewBag(views);

            graph.Actions().Each(a => AttemptToAttachViewToAction(bag, a));
        }

        public void AddFacility(IViewFacility facility)
        {
            _facilities.Add(facility);
        }

        public void AddViewsForActionFilter(IViewsForActionFilter filter)
        {
            _filters.Add(filter);
        }


        public void AttemptToAttachViewToAction(ViewBag bag, ActionCall call)
        {
            foreach (var filter in _filters)
            {
                var viewTokens = filter.Apply(call, bag);
                // if the filter returned more than one, consider it "failed", ignore it, and move on to the next
                if (viewTokens.Count() == 1)
                {
                    call.Append(viewTokens.First().ToBehavioralNode());
                    break;
                }
            }
        }
    }
}