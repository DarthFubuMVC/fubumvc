using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Tests.ServiceBus;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    [TestFixture]
    public class throws_descriptive_exception_if_no_incoming_lq_channel
    {
        [Test]
        public void should_throw_invalid_transport_exception_because_there_are_no_incoming_lq_transports()
        {
            var message =
                Exception<FubuException>.ShouldBeThrownBy(
                    () => FubuRuntime.For<BadTransportRegistry>()).Message;

            message.ShouldContain("You must have at least one incoming Lightning Queue channel for accepting replies");
        }
    }

    public class BadTransportRegistry : FubuTransportRegistry<BadTransportSettings>
    {
        public BadTransportRegistry()
        {
            Channel(x => x.Something).AcceptsMessage<Message1>();
        }
    }

    public class BadTransportSettings
    {
        public Uri Something { get; set; }
        public Uri Else { get; set; }
    }
}