using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Logging
{
    [TestFixture]
    public class EventAggregationListenerTester : InteractionContext<EventAggregationListener>
    {
        [Test]
        public void listens_to_events_in_FubuTransportation()
        {
            ClassUnderTest.ListensFor(typeof(ChainExecutionFinished)).ShouldBeTrue();
            ClassUnderTest.ListensFor(typeof(ChainExecutionStarted)).ShouldBeTrue();
        }

        [Test]
        public void does_not_listen_to_events_outside_of_FubuTransportation()
        {
            ClassUnderTest.ListensFor(GetType()).ShouldBeFalse();
        }

        [Test]
        public void send_debug_message()
        {
            var message = new ChainExecutionFinished();

            ClassUnderTest.DebugMessage(message);

            MockFor<IEventAggregator>().AssertWasCalled(x => x.SendMessage(message));
        }


        [Test]
        public void send_info_message()
        {
            var message = new ChainExecutionFinished();

            ClassUnderTest.InfoMessage(message);

            MockFor<IEventAggregator>().AssertWasCalled(x => x.SendMessage(message));
        }

        [Test]
        public void debug_disabled()
        {
            Services.Inject(new TransportSettings
            {
                DebugEnabled = false
            });

            ClassUnderTest.IsDebugEnabled.ShouldBeFalse();
        }

        [Test]
        public void debug_enabled()
        {
            Services.Inject(new TransportSettings
            {
                DebugEnabled = true
            });

            ClassUnderTest.IsDebugEnabled.ShouldBeTrue();
        }


    }

}