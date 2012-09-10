using System.Net;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Continuations
{
    public interface IContinuationDirector
    {
        void InvokeNextBehavior();
        void RedirectTo(object input, string categoryOrHttpMethod = null);
        void RedirectToCall(ActionCall call, string categoryOrHttpMethod = null);
        void TransferTo(object input, string categoryOrHttpMethod = null);
        void TransferToCall(ActionCall call, string categoryOrHttpMethod = null);
        void EndWithStatusCode(HttpStatusCode code);
    }
}