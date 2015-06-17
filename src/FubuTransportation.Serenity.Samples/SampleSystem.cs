using FubuTransportation.Configuration;
using FubuTransportation.Serenity.Samples.SystemUnderTest;

namespace FubuTransportation.Serenity.Samples
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