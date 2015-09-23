using System.Diagnostics;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Dates;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Tests.ServiceBus;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.LightningQueues.Testing
{
    [TestFixture]
    public class Full_end_to_end_delayed_message_processing_with_Lightning_Queues
    {
        private FubuRuntime _runtime;
        private IServiceBus theServiceBus;
        private SettableClock theClock;
        private OneMessage message1;
        private OneMessage message2;
        private OneMessage message3;
        private OneMessage message4;

        [TestFixtureSetUp]
        public void SetUp()
        {
            // Need to do something about this.  Little ridiculous
            var settings = new BusSettings
            {
                Downstream = "lq.tcp://localhost:2050/downstream".ToUri()
            };


            theClock = new SettableClock();


            _runtime = FubuRuntime.For<DelayedRegistry>(_ =>
            {
                _.Services.ReplaceService(settings);
                _.Services.ReplaceService<ISystemTime>(theClock);
            });

            theServiceBus = _runtime.Get<IServiceBus>();
            //_runtime.Get<IPersistentQueues>().ClearAll();

            message1 = new OneMessage();
            message2 = new OneMessage();
            message3 = new OneMessage();
            message4 = new OneMessage();

            Debug.WriteLine("The current Utc time is " + theClock.UtcNow());

            theServiceBus.DelaySend(message1, theClock.UtcNow().AddHours(1));
            theServiceBus.DelaySend(message2, theClock.UtcNow().AddHours(1));
            theServiceBus.DelaySend(message3, theClock.UtcNow().AddHours(2));
            theServiceBus.DelaySend(message4, theClock.UtcNow().AddHours(2));
        }

        [Test, Explicit("This is being problematic. Going to replace w/ an ST spec")]
        public void things_are_received_at_the_right_times()
        {
            TestMessageRecorder.AllProcessed.Any().ShouldBeFalse();

            Thread.Sleep(500);
            TestMessageRecorder.AllProcessed.Any().ShouldBeFalse();

            theClock.LocalNow(theClock.LocalTime().Add(61.Minutes()));

            Wait.Until(() => TestMessageRecorder.HasProcessed(message1)).ShouldBeTrue();
            Wait.Until(() => TestMessageRecorder.HasProcessed(message2)).ShouldBeTrue();

            TestMessageRecorder.HasProcessed(message3).ShouldBeFalse();
            TestMessageRecorder.HasProcessed(message4).ShouldBeFalse();

            theClock.LocalNow(theClock.LocalTime().Add(61.Minutes()));

            Wait.Until(() => TestMessageRecorder.HasProcessed(message3)).ShouldBeTrue();
            Wait.Until(() => TestMessageRecorder.HasProcessed(message4)).ShouldBeTrue();

            // If it's more than this, we got problems
            TestMessageRecorder.AllProcessed.Count().ShouldBe(4);
        }


        [TestFixtureTearDown]
        public void TearDown()
        {
            _runtime.Dispose();
        }
    }

    public class DelayedRegistry : FubuTransportRegistry<BusSettings>
    {
        public DelayedRegistry()
        {
            Handlers.DisableDefaultHandlerSource();
            ServiceBus.EnableInMemoryTransport();

            // Need this to be fast for the tests
            AlterSettings<TransportSettings>(x => x.DelayMessagePolling = 100);

            Services.ReplaceService<ISystemTime>(new SettableClock());
            Handlers.Include<SimpleHandler<OneMessage>>();
            Channel(x => x.Downstream).ReadIncoming().AcceptsMessagesInAssemblyContainingType<OneMessage>();
        }
    }
}