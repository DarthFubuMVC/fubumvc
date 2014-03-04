using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.Attachment
{
    [Title("Output type matches")]
    public class ActionReturnsViewModelType : IViewsForActionFilter
    {
        public IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views)
        {
            return views.ViewsFor(call.OutputType());
        }
    }
}