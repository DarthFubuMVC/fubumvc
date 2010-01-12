using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Continuations
{
    public interface IContinuationDirector
    {
        void InvokeNextBehavior();
        void RedirectTo(object input);
        void RedirectToCall(ActionCall call);
        void TransferTo(object input);
        void TransferToCall(ActionCall call);
    }
}