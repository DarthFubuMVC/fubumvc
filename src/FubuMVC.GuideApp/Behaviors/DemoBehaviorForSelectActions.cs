using FubuMVC.Core.Behaviors;

namespace FubuMVC.GuideApp.Behaviors
{
    public class DemoBehaviorForSelectActions : IActionBehavior
    {
        private readonly IActionBehavior _innerBehavior;

        public DemoBehaviorForSelectActions(IActionBehavior innerBehavior)
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