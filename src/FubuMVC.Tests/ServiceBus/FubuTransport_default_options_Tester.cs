using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class FubuTransport_default_options_Tester
    {
        [SetUp]
        public void SetUp()
        {
            FubuMode.RemoveTestingMode();
        }

        [Test]
        public void synchronous_event_aggregation_is_defaulted_to_false()
        {
            FubuTransport.UseSynchronousLogging = true;
            FubuTransport.Reset();

            FubuTransport.UseSynchronousLogging.ShouldBeFalse();
        }

        [Test]
        public void message_watching_is_defaulted_to_false()
        {
            FubuTransport.ApplyMessageHistoryWatching = true;
            FubuTransport.Reset();

            FubuTransport.ApplyMessageHistoryWatching.ShouldBeFalse();
        }


        [Test]
        public void use_in_memory_queues_is_defaulted_to_false()
        {
            FubuTransport.AllQueuesInMemory = true;
            FubuTransport.Reset();

            FubuTransport.AllQueuesInMemory.ShouldBeFalse();
        }

    }
}