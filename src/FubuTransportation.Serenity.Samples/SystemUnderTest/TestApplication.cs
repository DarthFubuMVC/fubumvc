using FubuMVC.Core;
using FubuTransportation.Configuration;
using StructureMap;

namespace FubuTransportation.Serenity.Samples.SystemUnderTest
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