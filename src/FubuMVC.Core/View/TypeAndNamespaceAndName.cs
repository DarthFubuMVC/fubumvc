using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    public class TypeAndNamespaceAndName : IViewAttachmentStrategy
    {
        public IViewToken Find(ActionCall call, ViewBag views)
        {
            return
                views
                    .ViewsFor(call.OutputType())
                    .Where(view => view.ViewType.Name == call.Method.Name 
                                   && view.ViewType.Namespace == call.HandlerType.Namespace)
                    .FirstOrDefault();
        }
    }
}