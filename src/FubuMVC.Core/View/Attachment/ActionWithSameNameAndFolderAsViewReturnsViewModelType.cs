using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.Attachment
{
    public class ActionWithSameNameAndFolderAsViewReturnsViewModelType : IViewsForActionFilter
    {
        public IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views)
        {
            return views.ViewsFor(call.OutputType()).Where(view => view.Name == call.Method.Name && view.Folder == call.HandlerType.Namespace);
        }
    }
}