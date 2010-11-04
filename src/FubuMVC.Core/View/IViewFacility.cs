using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View
{
    public interface IViewFacility
    {
        IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph);
    }
}