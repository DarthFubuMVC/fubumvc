using System.Linq;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Invocation.Batching;
using FubuMVC.Tests.ServiceBus.ScenarioSupport;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class handling_a_batch_of_messages : InvocationContext
    {
        [Test]
        public void generic_handler_is_applied_at_end()
        {
            handler<OneHandler, TwoHandler, ThreeHandler, GenericHandler>();
            handler<MyBatchHandler>();

            var message1 = new OneMessage();
            var message2 = new TwoMessage();
            var message3 = new ThreeMessage();

            sendOneMessage(new MyBatch(message1, message2, message3));

            TestMessageRecorder.AllProcessed.Count().ShouldBe(6);
            TestMessageRecorder.AllProcessed[0].ShouldMatch<OneHandler>(message1);
            TestMessageRecorder.AllProcessed[1].ShouldMatch<GenericHandler>(message1);
            TestMessageRecorder.AllProcessed[2].ShouldMatch<TwoHandler>(message2);
            TestMessageRecorder.AllProcessed[3].ShouldMatch<GenericHandler>(message2);
            TestMessageRecorder.AllProcessed[4].ShouldMatch<ThreeHandler>(message3);
            TestMessageRecorder.AllProcessed[5].ShouldMatch<GenericHandler>(message3);
        }
    }

    public class MyBatch : BatchMessage
    {
        public MyBatch()
        {
        }

        public MyBatch(params object[] messages) : base(messages)
        {
        }
    }

    public class MyBatchHandler : BatchConsumer<MyBatch>
    {
        public MyBatchHandler(IMessageExecutor executor) : base(executor)
        {
        }


    }
}