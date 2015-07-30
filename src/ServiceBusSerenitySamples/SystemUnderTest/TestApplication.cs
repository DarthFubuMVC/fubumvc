using FubuMVC.Core;

namespace ServiceBusSerenitySamples.SystemUnderTest
{
    public class TestApplication : IApplicationSource
    {
        public FubuApplication BuildApplication(string directory)
        {
            return FubuApplication.For<TestRegistry>(x => x.Services.For<MessageRecorder>().Singleton());
        }
    }
}