using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    public class TypeAndNamespaceAndName : IViewAttachmentStrategy
    {
        public IEnumerable<IViewToken> Find(ActionCall call, ViewBag views)
        {
            return
                views.ViewsFor(call.OutputType()).Where(
                    view => { return view.Name == call.Method.Name && view.Namespace == call.HandlerType.Namespace; });
        }
    }
}