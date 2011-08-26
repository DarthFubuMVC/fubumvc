using FubuMVC.Core.Behaviors;

namespace FubuMVC.GuideApp.Behaviors
{
    public class DemoBehaviorForAllActions : IActionBehavior
    {
        private readonly IActionBehavior _innerBehavior;

        public DemoBehaviorForAllActions(IActionBehavior innerBehavior)
        {
            _innerBehavior = innerBehavior;
        }

        public void Invoke()
        {
            _innerBehavior.Invoke();
        }

        public void InvokePartial()
        {
            _innerBehavior.InvokePartial();
        }
    }
}