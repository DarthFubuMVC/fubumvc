using System;

namespace FubuMVC.Core.Behaviors
{
    /// <summary>
    /// Implement this contract to be able to take part in behavior chains
    /// </summary>
    // SAMPLE: IActionBehavior
    public interface IActionBehavior
    {
        void Invoke();
        void InvokePartial();
    }
    // ENDSAMPLE
}