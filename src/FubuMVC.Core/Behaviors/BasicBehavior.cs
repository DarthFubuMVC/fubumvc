using System;
using System.Threading.Tasks;

namespace FubuMVC.Core.Behaviors
{
    public abstract class BasicBehavior : IActionBehavior
    {
        private readonly PartialBehavior _partialBehavior;
        private readonly Func<Task> _innerInvoke;

        protected BasicBehavior(PartialBehavior partialBehavior)
        {
            _partialBehavior = partialBehavior;


        }

        public IActionBehavior InsideBehavior { get; set; }

        public Task Invoke()
        {
            return InsideBehavior == null 
                ? invoke(() => Task.CompletedTask) 
                : invoke(() => InsideBehavior.Invoke());
        }

        private async Task invoke(Func<Task> inner)
        {
            var doNext = await performInvoke().ConfigureAwait(false);
            if (doNext == DoNext.Continue)
            {
                await inner().ConfigureAwait(false);
            }

            await afterInsideBehavior().ConfigureAwait(false);
        }

        public async Task InvokePartial()
        {
            Func<Task> inner = () => Task.CompletedTask;
            if (InsideBehavior != null)
            {
                inner = InsideBehavior.InvokePartial;
            }

            if (_partialBehavior == PartialBehavior.Executes)
            {
                await invoke(inner).ConfigureAwait(false);
            }
            else
            {
                await inner().ConfigureAwait(false);
            }
        }

        protected virtual Task<DoNext> performInvoke()
        {
            return Task.FromResult(DoNext.Continue);
        }

        protected virtual Task afterInsideBehavior()
        {
            return Task.CompletedTask;
        }
    }
}