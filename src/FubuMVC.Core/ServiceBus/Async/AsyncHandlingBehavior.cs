using System;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.ServiceBus.Async
{
    [Obsolete("Get rid of this")]
    public class AsyncHandlingBehavior : BasicBehavior
    {
        private readonly IAsyncHandling _asyncHandling;

        public AsyncHandlingBehavior(IAsyncHandling asyncHandling)
            : base(PartialBehavior.Executes)
        {
            _asyncHandling = asyncHandling;
        }

        protected override Task afterInsideBehavior()
        {
            throw new NotSupportedException("Get rid of me!");
        }
    }
}