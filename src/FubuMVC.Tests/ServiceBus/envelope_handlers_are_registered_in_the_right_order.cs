using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class envelope_handlers_are_registered_in_the_right_order
    {
        [Test]
        public void the_order()
        {
            using (var runtime = FubuTransport.For<Defaults>().Bootstrap())
            {
                var handlers = runtime.Factory.Get<IHandlerPipeline>().ShouldBeOfType<HandlerPipeline>().Handlers;


                handlers[0].ShouldBeOfType<DelayedEnvelopeHandler>();
                handlers[1].ShouldBeOfType<ResponseEnvelopeHandler>();
                handlers[2].ShouldBeOfType<ChainExecutionEnvelopeHandler>();
                handlers[3].ShouldBeOfType<NoSubscriberHandler>();
            }
        }

        public class Defaults : FubuTransportRegistry
        {
            public Defaults()
            {
                EnableInMemoryTransport();
            }
        }
    }
}