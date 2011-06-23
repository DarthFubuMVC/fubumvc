using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.Attachment
{
    public interface IViewBagConvention
    {
        void Configure(ViewBag bag, BehaviorGraph graph);
    }

    public class ViewBagConventionRunner : IViewAttacher
    {
        private readonly TypePool _types;
        private readonly List<IViewFacility> _facilities = new List<IViewFacility>();
        private readonly List<IViewBagConvention> _conventions = new List<IViewBagConvention>();

        public ViewBagConventionRunner(TypePool types)
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
            _conventions.Each(c => c.Configure(bag, graph));
        }

        public IEnumerable<IViewFacility> Facilities
        {
            get { return _facilities; }
        }

        public TypePool Types
        {
            get { return _types; }
        }

        public void Apply<TConvention>()
            where TConvention : IViewBagConvention, new()
        {
            Apply(new TConvention());
        }

        public void Apply(IViewBagConvention convention)
        {
            _conventions.Fill(convention);
        }

        public void AddFacility(IViewFacility facility)
        {
            var typeOfFacility = facility.GetType();
            if (_facilities.Any(f => f.GetType() == typeOfFacility)) return;
            _facilities.Add(facility);
        }
    }
}