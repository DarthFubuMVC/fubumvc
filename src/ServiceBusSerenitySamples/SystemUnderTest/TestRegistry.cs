using System;
using FubuMVC.Core.ServiceBus.Configuration;
using ServiceBusSerenitySamples.Setup;
using ServiceBusSerenitySamples.SystemUnderTest.Subscriptions;

namespace ServiceBusSerenitySamples.SystemUnderTest
{
    public class TestRegistry : FubuTransportRegistry<TestSettings>
    {
        public static bool InMemory;
        public TestRegistry()
        {
            Channel(x => x.SystemUnderTest)
                .AcceptsMessage<SomeCommand>()
                .AcceptsMessage<TestMessage>()
                .ReadIncoming();

            Channel(x => x.AnotherService)
                .AcceptsMessage<MessageForExternalService>();
        }
    }

    public class TestSettings
    {
        public Uri AnotherService { get; set; }
        public Uri SystemUnderTest { get; set; }
    }
}