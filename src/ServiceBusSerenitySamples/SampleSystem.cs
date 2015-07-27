using FubuMVC.Core.ServiceBus.Configuration;
using Serenity.ServiceBus;
using ServiceBusSerenitySamples.SystemUnderTest;

namespace ServiceBusSerenitySamples
{
    public class SampleSystem : FubuTransportSystem<TestApplication>
    {
        public SampleSystem()
        {
            OnContextCreation<SystemUnderTest.MessageRecorder>(x => x.Messages.Clear());
            FubuTransport.SetupForInMemoryTesting<TestSettings>();
        }
    }
}