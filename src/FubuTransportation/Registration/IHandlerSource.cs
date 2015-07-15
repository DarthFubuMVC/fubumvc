using System.Collections.Generic;
using System.Reflection;
using FubuTransportation.Registration.Nodes;

namespace FubuTransportation.Registration
{
    public interface IHandlerSource
    {
        IEnumerable<HandlerCall> FindCalls(Assembly applicationAssembly);
    }
}