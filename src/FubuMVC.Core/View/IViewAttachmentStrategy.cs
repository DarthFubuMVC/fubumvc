using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    public interface IViewAttachmentStrategy
    {
        IEnumerable<IViewToken> Find(ActionCall call, ViewBag views);
    }
}