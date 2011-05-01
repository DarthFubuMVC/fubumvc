using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.Attachment
{
    public interface IViewsForActionFilter
    {
        IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views);
    }
}