namespace FubuMVC.Core.Behaviors
{
    public interface IActionBehavior
    {
        void Invoke();
        void InvokePartial();
    }
}