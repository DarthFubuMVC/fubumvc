using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Core.View
{
    /// <summary>
    /// Implement this contract to provide a service which allows to obatin
    /// <see cref="IViewToken"/>s based on a <see cref="TypePool"/> and the
    /// relevant <see cref="BehaviorGraph"/>
    /// </summary>
    public interface IViewFacility
    {
        Task<IEnumerable<ITemplateFile>> FindViews(BehaviorGraph graph);
    }
}