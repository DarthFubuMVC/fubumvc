using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Continuations
{
    /// <summary>
    /// Use to process an adhoc FubuContinuation in a service or
    /// custom behavior
    /// </summary>
    public interface IContinuationProcessor
    {
        void Continue(FubuContinuation continuation, IActionBehavior nextBehavior);
    }
}