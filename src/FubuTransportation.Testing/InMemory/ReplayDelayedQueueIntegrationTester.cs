using System;
using System.Threading;
using FubuCore.Dates;
using FubuMVC.Core;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Services.Messaging.Tracking;
using FubuMVC.Core.StructureMap;
using FubuTransportation.Configuration;
using FubuTransportation.InMemory;
using FubuTransportation.Polling;
using FubuTransportation.Runtime.Delayed;
using FubuTransportation.Testing.ScenarioSupport;
using NUnit.Framework;
using StructureMap;
using System.Collections.Generic;
using FubuCore;
using System.Linq;
using FubuTestingSupport;

namespace FubuTransportation.Testing.InMemory
{
    [TestFixture]
    public class ReplayDelayedQueueIntegrationTester
    {
        private FubuRuntime theRuntime;
        private IServiceBus theServiceBus;
        private SettableClock theClock;
        private OneMessage message1;
        private OneMessage message2;
        private OneMessage message3;
        private OneMessage message4;
        private DelayedEnvelopeProcessor theProcessor;

        [TestFixtureSetUp]
        public void SetUp()
        {
            // Need to do something about this.  Little ridiculous
            FubuTransport.SetupForInMemoryTesting();
            TestMessageRecorder.Clear();
            MessageHistory.ClearAll();
            InMemoryQueueManager.ClearAll();

            theRuntime = FubuTransport.For<DelayedRegistry>()
                                       .Bootstrap();

            // Disable polling!
            theRuntime.Factory.Get<IPollingJobs>().Each(x => x.Stop());

            theServiceBus = theRuntime.Factory.Get<IServiceBus>();

            theClock = theRuntime.Factory.Get<ISystemTime>().As<SettableClock>();

            message1 = new OneMessage();
            message2 = new OneMessage();
            message3 = new OneMessage();
            message4 = new OneMessage();

            theServiceBus.DelaySend(message1, theClock.UtcNow().AddHours(1));
            theServiceBus.DelaySend(message2, theClock.UtcNow().AddHours(1));
            theServiceBus.DelaySend(message3, theClock.UtcNow().AddHours(2));
            theServiceBus.DelaySend(message4, theClock.UtcNow().AddHours(2));

            theProcessor = theRuntime.Factory.Get<DelayedEnvelopeProcessor>();
        }

        [Test]
        public void things_are_received_at_the_right_times()
        {
            TestMessageRecorder.AllProcessed.Any().ShouldBeFalse();

            theProcessor.Execute(new CancellationToken());
            Thread.Sleep(2000);
            TestMessageRecorder.AllProcessed.Any().ShouldBeFalse();

            theClock.LocalNow(theClock.LocalTime().Add(61.Minutes()));
            theProcessor.Execute(new CancellationToken());

            Wait.Until(() => TestMessageRecorder.HasProcessed(message1)).ShouldBeTrue();
            Wait.Until(() => TestMessageRecorder.HasProcessed(message2)).ShouldBeTrue();

            TestMessageRecorder.HasProcessed(message3).ShouldBeFalse();
            TestMessageRecorder.HasProcessed(message4).ShouldBeFalse();

            theClock.LocalNow(theClock.LocalTime().Add(61.Minutes()));
            theProcessor.Execute(new CancellationToken());

            Wait.Until(() => TestMessageRecorder.HasProcessed(message3)).ShouldBeTrue();
            Wait.Until(() => TestMessageRecorder.HasProcessed(message4)).ShouldBeTrue();

            // If it's more than this, we got problems
            TestMessageRecorder.AllProcessed.Count().ShouldEqual(4);
        }


        [TestFixtureTearDown]
        public void TearDown()
        {
            theRuntime.Dispose();
            FubuTransport.Reset();
        }
    }

    public class DelayedRegistry : FubuTransportRegistry<BusSettings>
    {
        public DelayedRegistry()
        {
            Handlers.DisableDefaultHandlerSource();
            EnableInMemoryTransport();

            // Need this to be fast for the tests
            AlterSettings<TransportSettings>(x => x.DelayMessagePolling = 100);

            Services(x => x.ReplaceService<ISystemTime>(new SettableClock()));
            Handlers.Include<SimpleHandler<OneMessage>>();
            Channel(x => x.Downstream).ReadIncoming().AcceptsMessagesInAssemblyContainingType<OneMessage>();
        }
    }
}