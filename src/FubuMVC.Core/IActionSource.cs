using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    public interface IActionSource
    {
        IEnumerable<ActionCall> FindActions(TypePool types);
    }
}