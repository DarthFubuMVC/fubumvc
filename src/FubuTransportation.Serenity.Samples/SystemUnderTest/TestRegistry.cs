using System;
using FubuTransportation.Configuration;
using FubuTransportation.Serenity.Samples.Setup;
using FubuTransportation.Serenity.Samples.SystemUnderTest.Subscriptions;

namespace FubuTransportation.Serenity.Samples.SystemUnderTest
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