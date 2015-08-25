using System.Collections.Generic;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics.Instrumentation
{
    [TestFixture]
    public class VerboseExecutionLoggerTester : InteractionContext<VerboseExecutionLogger>
    {
        [Test]
        public void record()
        {
            var log = MockFor<IChainExecutionLog>();
            var env = new Dictionary<string, object>();

            ClassUnderTest.Record(log, env);

            log.AssertWasCalled(x => x.RecordHeaders(env));
            log.AssertWasCalled(x => x.RecordBody(env));
        
            MockFor<IExecutionLogStorage>().AssertWasCalled(x => x.Store(log));
        }
    }
}