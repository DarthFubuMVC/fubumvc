using System;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus
{
    public class TestEnvelopeContext : EnvelopeContext
    {
        public TestEnvelopeContext() : this(MockRepository.GenerateMock<IHandlerPipeline>())
        {
            HandlerPipeline.Stub(x => x.Invoke(null, null)).IgnoreArguments().Return(Task.CompletedTask);
        }

        public TestEnvelopeContext(IHandlerPipeline handlerPipeline)
            : base(
                new RecordingLogger(), new SettableClock(), MockRepository.GenerateMock<IChainInvoker>(),
                new RecordingEnvelopeSender(), handlerPipeline)
        {
            SystemTime.As<SettableClock>().LocalNow(DateTime.Today.AddHours(5));

            HandlerPipeline = handlerPipeline;
        }

        public IHandlerPipeline HandlerPipeline { get; set; }

        public RecordingEnvelopeSender RecordedOutgoing => Outgoing.As<RecordingEnvelopeSender>();

        public RecordingLogger RecordedLogs => logger.As<RecordingLogger>();
    }
}