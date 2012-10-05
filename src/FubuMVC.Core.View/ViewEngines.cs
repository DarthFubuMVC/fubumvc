using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.View
{
    public class ViewEngines
    {
        private BehaviorGraph _graph;
        private readonly IList<ViewTokenPolicy> _viewPolicies = new List<ViewTokenPolicy>();

        private readonly List<IViewFacility> _facilities = new List<IViewFacility>();
        private readonly Lazy<ViewBag> _viewBag;

        public ViewEngines()
        {
            _viewBag = new Lazy<ViewBag>(buildViewBag);
        }

        // Testing constructor
        public ViewEngines(IEnumerable<IViewToken> theViews)
        {
            _viewBag = new Lazy<ViewBag>(() => new ViewBag(theViews));
        }

        public void UseGraph(BehaviorGraph graph)
        {
            _graph = graph;
        }

        /// <summary>
        ///   Define a view activation policy for views matching the filter.
        ///   <seealso cref = "IfTheInputModelOfTheViewMatches" />
        /// </summary>
        public PageActivationExpression IfTheViewMatches(Func<IViewToken, bool> filter)
        {
            return new PageActivationExpression(this, filter);
        }

        /// <summary>
        ///   Define a view activation policy by matching on the input type of a view.
        ///   A view activation element implements <see cref = "IPageActivationAction" /> and takes part in setting up a View instance correctly
        ///   at runtime.
        /// </summary>
        public PageActivationExpression IfTheInputModelOfTheViewMatches(Func<Type, bool> filter)
        {
            Func<IViewToken, bool> combined = viewToken => { return filter(viewToken.ViewModel); };

            return IfTheViewMatches(combined);
        }

        public ViewBag Views {get { return _viewBag.Value; }}

        private ViewBag buildViewBag()
        {
            var views = new List<IViewToken>();

            foreach (var facility in _facilities)
            {
                views.AddRange(facility.FindViews(_graph));
            }

            _viewPolicies.Each(x => x.Alter(views));

            return new ViewBag(views);
        }

        public IEnumerable<IViewFacility> Facilities
        {
            get { return _facilities; }
        }

        public void AddFacility(IViewFacility facility)
        {
            var typeOfFacility = facility.GetType();
            if (_facilities.Any(f => f.GetType() == typeOfFacility)) return;

            _facilities.Add(facility);
        }

        public void AddPolicy(ViewTokenPolicy policy)
        {
            _viewPolicies.Add(policy);
        }
    }

    public class ViewTokenPolicy : DescribesItself
    {
        private readonly Func<IViewToken, bool> _filter;
        private readonly Action<IViewToken> _alteration;
        private readonly string _description;

        public ViewTokenPolicy(Func<IViewToken, bool> filter, Action<IViewToken> alteration, string description)
        {
            _filter = filter;
            _alteration = alteration;
            _description = description;
        }

        public void Describe(Description description)
        {
            description.Title = _description;
        }

        public void Alter(IEnumerable<IViewToken> views)
        {
            views.Where(_filter).Each(_alteration);
        }

        public override string ToString()
        {
            return string.Format("ViewTokenPolicy: {0}", _description);
        }
    }
}