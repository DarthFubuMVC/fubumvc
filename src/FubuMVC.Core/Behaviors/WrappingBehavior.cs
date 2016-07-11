using System;
using System.Threading.Tasks;

namespace FubuMVC.Core.Behaviors
{
    public abstract class WrappingBehavior : IActionBehavior
    {
        public IActionBehavior Inner { get; set; }

        public async Task Invoke()
        {
            if (Inner != null)
            {
                await invoke(() => Inner.Invoke()).ConfigureAwait(false);
            }
        }

        public async Task InvokePartial()
        {
            if (Inner != null)
            {
                await invoke(() => Inner.InvokePartial()).ConfigureAwait(false);
            }
        }

        protected abstract Task invoke(Func<Task> func);
    }
}