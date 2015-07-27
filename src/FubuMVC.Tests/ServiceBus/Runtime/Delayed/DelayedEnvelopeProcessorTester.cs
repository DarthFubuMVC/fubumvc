using System;
using System.Collections.Generic;
using System.Threading;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Delayed
{
    [TestFixture]
    public class DelayedEnvelopeProcessorTester : InteractionContext<DelayedEnvelopeProcessor>
    {
        [Test]
        public void dequeue_from_all_the_transports()
        {
            var transports = Services.CreateMockArrayFor<ITransport>(4);

            LocalSystemTime = DateTime.Today.AddHours(5);

            ClassUnderTest.Execute(new CancellationToken());

            transports.Each(transport => {
                transport.AssertWasCalled(x => x.ReplayDelayed(UtcSystemTime));
            });
        }

        [Test]
        public void dequeue_a_single_transport_should_log_all_the_requeued_envelopes()
        {
            var logger = new RecordingLogger();
            Services.Inject<ILogger>(logger);

            var envelopes = new EnvelopeToken[] {new EnvelopeToken(), new EnvelopeToken(), new EnvelopeToken()};
            LocalSystemTime = DateTime.Today.AddHours(5);
            var theTransport = MockFor<ITransport>();

            theTransport.Stub(x => x.ReplayDelayed(UtcSystemTime))
                        .Return(envelopes);

            ClassUnderTest.DequeuFromTransport(theTransport, UtcSystemTime);

            envelopes.Each(env => {
                logger.InfoMessages.ShouldContain(new DelayedEnvelopeAddedBackToQueue{Envelope = env});
            });
        }
    }

}