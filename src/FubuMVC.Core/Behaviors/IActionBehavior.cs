using System;

namespace FubuMVC.Core.Behaviors
{
    /// <summary>
    /// Implement this contract to be able to take part in behavior chains
    /// </summary>
    public interface IActionBehavior
    {
        void Invoke();
        void InvokePartial();
    }

    /// <summary>
    /// Serves as the entry behavior for a chain.
    /// </summary>
    public interface IEntrypointActionBehavior : IActionBehavior, IDisposable
    {
    }
}