using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    public class TypeAndNamespace : IViewAttachmentStrategy
    {
        public IViewToken Find(ActionCall call, ViewBag views)
        {
            return 
                views
                    .ViewsFor(call.OutputType())
                    .Where(view => view.ViewType.Namespace == call.HandlerType.Namespace)
                    .FirstOrDefault();
        }
    }

    public class UniqueTypeMatcher : IViewAttachmentStrategy
    {
        public IViewToken Find(ActionCall call, ViewBag views)
        {
            return views
                .ViewsFor(call.OutputType())
                .FirstOrDefault();
        }
    }
}