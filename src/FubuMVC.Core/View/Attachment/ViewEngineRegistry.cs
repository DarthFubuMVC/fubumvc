using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.Attachment
{
    public class ViewEngineRegistry : IViewEngineRegistry
    {
        private readonly List<IViewFacility> _facilities = new List<IViewFacility>();

        public ViewBag BuildViewBag()
        {
            var views = new List<IViewToken>();

            foreach (var facility in _facilities)
            {
                views.AddRange(facility.FindViews());
            }

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
    }
}