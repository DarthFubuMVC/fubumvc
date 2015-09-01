using System;
using System.Linq;
using FubuCore.Dates;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class DelayedEnvelopeHandlerTester
    {
        [Test]
        public void matches_positive()
        {
            var systemTime = SystemTime.Default();
            var envelope = new Envelope();
            envelope.ExecutionTime = systemTime.UtcNow().AddHours(1);

            var handler = new DelayedEnvelopeHandler(systemTime);

            envelope.IsDelayed(systemTime.UtcNow()).ShouldBeTrue();
            handler.Matches(envelope).ShouldBeTrue();
        }

        [Test]
        public void matches_negative_with_no_execution_time_header()
        {
            var systemTime = SystemTime.Default();
            var envelope = new Envelope();

            var handler = new DelayedEnvelopeHandler(systemTime);

            envelope.IsDelayed(systemTime.UtcNow()).ShouldBeFalse();
            handler.Matches(envelope).ShouldBeFalse();
        }

        [Test]
        public void matches_negative_when_the_execution_time_is_in_the_past()
        {
            var systemTime = SystemTime.Default();
            var envelope = new Envelope();
            envelope.ExecutionTime = systemTime.UtcNow().AddHours(-1);

            var handler = new DelayedEnvelopeHandler(systemTime);

            envelope.IsDelayed(systemTime.UtcNow()).ShouldBeFalse();
            handler.Matches(envelope).ShouldBeFalse();
        }

        [Test]
        public void execute_happy_path()
        {
            var context = new TestEnvelopeContext();
            var envelope = ObjectMother.Envelope();
            envelope.ExecutionTime = DateTime.Today;

            new DelayedEnvelopeHandler(null).Execute(envelope, context);

            envelope.Callback.AssertWasCalled(x => x.MoveToDelayedUntil(envelope.ExecutionTime.Value));

            context.RecordedLogs.InfoMessages.Single().ShouldBeOfType<DelayedEnvelopeReceived>()
                  .Envelope.ShouldBe(envelope.ToToken());
        }

        [Test]
        public void execute_sad_path()
        {
            var context = new TestEnvelopeContext();
            var envelope = ObjectMother.Envelope();
            envelope.ExecutionTime = DateTime.Today;

            var exception = new NotImplementedException();
            envelope.Callback.Stub(x => x.MoveToDelayedUntil(envelope.ExecutionTime.Value)).Throw(exception);

            new DelayedEnvelopeHandler(SystemTime.Default()).Execute(envelope, context);

            envelope.Callback.AssertWasCalled(x => x.MarkFailed(exception));


            var report = context.RecordedLogs.ErrorMessages.Single().ShouldBeOfType<FubuCore.Logging.ExceptionReport>();
            report.CorrelationId.ShouldBe(envelope.CorrelationId);
            report.ExceptionText.ShouldBe(exception.ToString());

        }


    }
}