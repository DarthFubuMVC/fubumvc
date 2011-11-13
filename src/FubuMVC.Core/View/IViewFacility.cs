using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View
{
    /// <summary>
    /// Implement this contract to provide a service which allows to obatin
    /// <see cref="IViewToken"/>s based on a <see cref="TypePool"/> and the
    /// relevant <see cref="BehaviorGraph"/>
    /// </summary>
    public interface IViewFacility
    {
        IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph);
    }
}