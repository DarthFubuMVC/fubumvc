using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Continuations
{
    public class ContinuationProcessor : IContinuationProcessor
    {
        private readonly ContinuationHandler _handler;

        public ContinuationProcessor(ContinuationHandler handler)
        {
            _handler = handler;
        }

        public void Continue(FubuContinuation continuation, IActionBehavior nextBehavior)
        {
            _handler.InsideBehavior = nextBehavior;
            continuation.Process(_handler);
        }
    }
}