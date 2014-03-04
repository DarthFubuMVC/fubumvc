using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.Attachment
{
    [Title("Action name matches view name, same namespace/folder, same output type")]
    public class ActionWithSameNameAndFolderAsViewReturnsViewModelType : IViewsForActionFilter
    {
        public IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views)
        {
            return views.ViewsFor(call.OutputType()).Where(view => view.Name() == call.Method.Name && view.Namespace == call.HandlerType.Namespace);
        }
    }
}