using System.Collections.Generic;
using FubuTransportation.Registration.Nodes;

namespace FubuTransportation.Registration
{
    public interface IHandlerSource
    {
        IEnumerable<HandlerCall> FindCalls();
    }
}