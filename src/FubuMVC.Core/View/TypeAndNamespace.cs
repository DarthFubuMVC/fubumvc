using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    public class TypeAndNamespace : IViewAttachmentStrategy
    {
        public IEnumerable<IViewToken> Find(ActionCall call, ViewBag views)
        {
            return views.ViewsFor(call.OutputType()).Where(view => view.Folder == call.HandlerType.Namespace);
        }
    }

    public class UniqueTypeMatcher : IViewAttachmentStrategy
    {
        public IEnumerable<IViewToken> Find(ActionCall call, ViewBag views)
        {
            return views.ViewsFor(call.OutputType());
        }
    }
}