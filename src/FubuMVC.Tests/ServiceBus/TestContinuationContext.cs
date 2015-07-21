using System;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Rhino.Mocks;

namespace FubuTransportation.Testing
{
    public class TestContinuationContext : ContinuationContext
    {
        public TestContinuationContext()
            : base(
                new RecordingLogger(), new SettableClock(), MockRepository.GenerateMock<IChainInvoker>(),
                new RecordingEnvelopeSender())
        {
            SystemTime.As<SettableClock>().LocalNow(DateTime.Today.AddHours(5));
        }

        public RecordingEnvelopeSender RecordedOutgoing
        {
            get { return Outgoing.As<RecordingEnvelopeSender>(); }
        }

        public RecordingLogger RecordedLogs
        {
            get { return Logger.As<RecordingLogger>(); }
        }
    }
}