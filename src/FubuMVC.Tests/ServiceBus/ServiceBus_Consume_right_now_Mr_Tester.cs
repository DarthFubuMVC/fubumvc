using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.StructureMap;
using FubuMVC.Tests.ServiceBus.ScenarioSupport;
using NUnit.Framework;
using StructureMap;
using System.Linq;
using FubuTestingSupport;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class ServiceBus_Consume_right_now_Tester
    {
        [Test]
        public void send_now_is_handled_right_now()
        {

            using (var runtime = FubuTransport.For(x =>
            {
                x.EnableInMemoryTransport();
                x.Handlers.DisableDefaultHandlerSource();
                x.Handlers.Include<SimpleHandler<OneMessage>>();
            }).Bootstrap())
            {
                var serviceBus = runtime.Factory.Get<IServiceBus>();

                TestMessageRecorder.Clear();

                var message = new OneMessage();

                serviceBus.Consume(message);

                TestMessageRecorder.ProcessedFor<OneMessage>().Single().Message
                    .ShouldBeTheSameAs(message);
            }
        }
    }
}