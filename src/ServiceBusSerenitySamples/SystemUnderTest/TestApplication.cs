using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using StructureMap;

namespace ServiceBusSerenitySamples.SystemUnderTest
{
    public class TestApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            var container = new Container(x =>
            {
                x.For<MessageRecorder>().Singleton();
            });

            return FubuTransport.For<TestRegistry>(container);
        }
    }
}