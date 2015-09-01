using System;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus
{
    public class TestEnvelopeContext : EnvelopeContext
    {
        public TestEnvelopeContext()
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
            get { return logger.As<RecordingLogger>(); }
        }
    }
}