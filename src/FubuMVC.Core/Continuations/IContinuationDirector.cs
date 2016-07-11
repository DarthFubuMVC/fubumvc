using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Continuations
{
    public interface IContinuationDirector
    {
        Task InvokeNextBehavior();
        Task RedirectTo(object input, string categoryOrHttpMethod = null);
        Task RedirectToCall(ActionCall call, string categoryOrHttpMethod = null);
        Task TransferTo(object input, string categoryOrHttpMethod = null);
        Task TransferToCall(ActionCall call, string categoryOrHttpMethod = null);
        Task EndWithStatusCode(HttpStatusCode code);
    }
}