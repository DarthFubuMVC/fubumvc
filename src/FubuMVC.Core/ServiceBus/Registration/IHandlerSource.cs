using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus.Registration.Nodes;

namespace FubuMVC.Core.ServiceBus.Registration
{
    public interface IHandlerSource
    {
        Task<HandlerCall[]> FindCalls(Assembly applicationAssembly);
    }
}