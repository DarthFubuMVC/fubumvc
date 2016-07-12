using System.Threading.Tasks;
using FubuCore.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IHandlerPipeline
    {
        Task Invoke(Envelope envelope, IEnvelopeContext context);
        Task Receive(Envelope envelope);
        Task InvokeNow(Envelope envelope);
    }
}