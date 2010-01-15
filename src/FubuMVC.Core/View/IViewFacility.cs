using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View
{
    public interface IViewFacility
    {
        IEnumerable<IDiscoveredViewToken> FindViews(TypePool types);
    }
}