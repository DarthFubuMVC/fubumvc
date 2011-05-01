using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.Attachment
{
    public class ActionReturnsViewModelType : IViewsForActionFilter
    {
        public IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views)
        {
            return views.ViewsFor(call.OutputType());
        }
    }
}