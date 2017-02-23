using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    
    public class envelope_handlers_are_registered_in_the_right_order
    {
        [Fact]
        public void the_order()
        {
            using (var runtime = FubuRuntime.For<Defaults>())
            {
                var handlers = runtime.Get<IHandlerPipeline>().ShouldBeOfType<HandlerPipeline>().Handlers;


                handlers[0].ShouldBeOfType<DelayedEnvelopeHandler>();
                handlers[1].ShouldBeOfType<ResponseEnvelopeHandler>();
                handlers[2].ShouldBeOfType<ChainExecutionEnvelopeHandler>();
                handlers[3].ShouldBeOfType<NoSubscriberHandler>();
            }
        }

        public class Defaults : FubuRegistry
        {
            public Defaults()
            {
                ServiceBus.Enable(true);
                ServiceBus.EnableInMemoryTransport();
            }
        }
    }
}