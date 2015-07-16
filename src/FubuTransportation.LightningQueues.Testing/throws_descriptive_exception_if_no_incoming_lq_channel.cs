using System;
using FubuCore;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Testing;
using NUnit.Framework;

namespace FubuTransportation.LightningQueues.Testing
{
    [TestFixture]
    public class throws_descriptive_exception_if_no_incoming_lq_channel
    {
        [Test]
        public void should_throw_invalid_transport_exception_because_there_are_no_incoming_lq_transports()
        {
            string message =
                Exception<FubuException>.ShouldBeThrownBy(
                    () => { FubuTransport.For<BadTransportRegistry>().Bootstrap(); }).Message;

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