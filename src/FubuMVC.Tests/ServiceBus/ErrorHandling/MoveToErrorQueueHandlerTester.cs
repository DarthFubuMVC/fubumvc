using System;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class MoveToErrorQueueHandlerTester
    {
        [Test]
        public void do_nothing_if_it_is_not_the_right_exception()
        {
            var handler = new MoveToErrorQueueHandler<NotImplementedException>();
            handler.DetermineContinuation(null, new Exception()).ShouldBeNull();
            handler.DetermineContinuation(null, new ApplicationException()).ShouldBeNull();
            handler.DetermineContinuation(null, new NotSupportedException()).ShouldBeNull();
        }

        [Test]
        public void moves_to_the_error_queue_if_the_exception_matches()
        {
            var handler = new MoveToErrorQueueHandler<NotImplementedException>();
            var ex = new NotImplementedException();

            handler.DetermineContinuation(null, ex).ShouldBeOfType<MoveToErrorQueue>()
                .Exception.ShouldBeTheSameAs(ex);
        }
    }
}