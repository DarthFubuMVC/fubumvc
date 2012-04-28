using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.Attachment
{
    public interface IViewEngineRegistry
    {
        IEnumerable<IViewFacility> Facilities { get; }
        void AddFacility(IViewFacility facility);
        ViewBag BuildViewBag();
    }
}