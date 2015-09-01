using System;
using System.ComponentModel;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.Async
{
    [Description("Executes the chain asynchronously in a different thread than the original dequeue-ing thread")]
    public class AsyncChainExecutionContinuation : IContinuation
    {
        private readonly Func<IContinuation> _inner;
        private Task _task;

        public AsyncChainExecutionContinuation(Func<IContinuation> inner)
        {
            _inner = inner;
        }

        public void Execute(Envelope envelope, IEnvelopeContext context)
        {
            _task = Task.Factory.StartNew(() => {
                var continuation = _inner();
                continuation.Execute(envelope, context);
            });
        }

        public Task Task
        {
            get { return _task; }
        }
    }
}