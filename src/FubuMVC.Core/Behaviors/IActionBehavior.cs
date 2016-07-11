using System;
using System.Threading.Tasks;

namespace FubuMVC.Core.Behaviors
{
    /// <summary>
    /// Implement this contract to be able to take part in behavior chains
    /// </summary>
    // SAMPLE: IActionBehavior
    public interface IActionBehavior
    {
        Task Invoke();
        Task InvokePartial();
    }
    // ENDSAMPLE
}