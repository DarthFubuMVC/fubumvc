using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public interface IActionSource
    {
        IEnumerable<ActionCall> FindActions(TypePool types);
    }
}