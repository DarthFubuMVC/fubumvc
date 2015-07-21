using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.ServiceBus.Registration.Nodes;

namespace FubuMVC.Core.ServiceBus.Registration
{
    public interface IHandlerSource
    {
        IEnumerable<HandlerCall> FindCalls(Assembly applicationAssembly);
    }
}