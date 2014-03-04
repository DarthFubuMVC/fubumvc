using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.Attachment
{
    /// <summary>
    /// Implement this contract to specify your own strategy how <see cref="IViewToken"/>s
    /// are found and attached for a given <see cref="ActionCall"/> .
    /// </summary>
    public interface IViewsForActionFilter
    {
        IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views);
    }
}