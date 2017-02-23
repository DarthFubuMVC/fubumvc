using System.Collections.Generic;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics.Instrumentation
{
    
    public class ProductionExecutionLoggerTester : InteractionContext<ProductionExecutionLogger>
    {
        [Fact]
        public void record()
        {
            var log = MockFor<IChainExecutionLog>();
            var env = new Dictionary<string, object>();

            ClassUnderTest.Record(log, env);

            log.AssertWasCalled(x => x.RecordHeaders(env));
            log.AssertWasNotCalled(x => x.RecordBody(env));

            MockFor<IExecutionLogStorage>().AssertWasCalled(x => x.Store(log));
        }

        [Fact]
        public void record_for_envelope()
        {
            var log = MockFor<IChainExecutionLog>();
            var envelope = new Envelope();

            ClassUnderTest.Record(log, envelope);

            log.AssertWasCalled(x => x.RecordHeaders(envelope));
            log.AssertWasNotCalled(x => x.RecordBody(envelope));

            MockFor<IExecutionLogStorage>().AssertWasCalled(x => x.Store(log));
        }
    }
}