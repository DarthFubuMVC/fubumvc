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
        private readonly List<IViewAttachmentStrategy> _strategies = new List<IViewAttachmentStrategy>();
        private readonly TypePool _types;

        public ViewAttacher(TypePool types)
        {
            _types = types;

            AddFacility(new WebFormViewFacility());
        }

        public void Configure(BehaviorGraph graph)
        {
            IEnumerable<IViewToken> views = _facilities.SelectMany(x => x.FindViews(_types));
            var bag = new ViewBag(views);

            graph.Actions().Each(a => { attachView(bag, a); });
        }

        public void AddFacility(IViewFacility facility)
        {
            _facilities.Add(facility);
        }

        public void AddAttachmentStrategy(IViewAttachmentStrategy strategy)
        {
            _strategies.Add(strategy);
        }


        private void attachView(ViewBag bag, ActionCall call)
        {
            foreach (IViewAttachmentStrategy strategy in _strategies)
            {
                IEnumerable<IViewToken> tokens = strategy.Find(call, bag);
                if (tokens.Count() == 1)
                {
                    IViewToken token = tokens.First();
                    call.Append(token.ToBehavioralNode());
                    break;
                }
            }
        }
    }
}