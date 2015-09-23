using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using NUnit.Framework;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class ServiceBus_Consume_right_now_Tester
    {
        [Test]
        public void send_now_is_handled_right_now()
        {

            using (var runtime = FubuRuntime.For<FubuRegistry>(x =>
            {
                x.ServiceBus.Enable(true);
                x.ServiceBus.EnableInMemoryTransport();
                x.Handlers.DisableDefaultHandlerSource();
                x.Handlers.Include<SimpleHandler<OneMessage>>();
            }))
            {
                var serviceBus = runtime.Get<IServiceBus>();

                TestMessageRecorder.Clear();

                var message = new OneMessage();

                serviceBus.Consume(message);

                TestMessageRecorder.ProcessedFor<OneMessage>().Single().Message
                    .ShouldBeTheSameAs(message);
            }
        }
    }
}