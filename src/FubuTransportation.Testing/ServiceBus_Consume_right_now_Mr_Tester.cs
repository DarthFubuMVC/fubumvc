using FubuMVC.Core.StructureMap;
using FubuTransportation.Configuration;
using FubuTransportation.Testing.ScenarioSupport;
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
            using (var container = new Container())
            {
                using (var runtime = FubuTransport.For(x => {
                    x.EnableInMemoryTransport();
                    x.Handlers.DisableDefaultHandlerSource();
                                           x.Handlers.Include<SimpleHandler<OneMessage>>();
                }).StructureMap(container).Bootstrap())

                {
                    var serviceBus = container.GetInstance<IServiceBus>();

                    TestMessageRecorder.Clear();

                    var message = new OneMessage();

                    serviceBus.Consume(message);

                    TestMessageRecorder.ProcessedFor<OneMessage>().Single().Message
                        .ShouldBeTheSameAs(message);
                }
            }
        }
    }
}