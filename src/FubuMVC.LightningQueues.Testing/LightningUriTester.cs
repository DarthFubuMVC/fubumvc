using System;
using Xunit;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    
    public class LightningUriTester
    {
        private LightningUri theUri = new LightningUri("lq.tcp://localhost:2424/some_queue");


        [Fact]
        public void blows_up_if_protocol_is_not_lightning_queues()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new LightningUri("foo://bar");
            });
        }

        [Fact]
        public void finds_the_port()
        {
            theUri.Port.ShouldBe(2424);
        }

        [Fact]
        public void parses_the_queue_name()
        {
            theUri.QueueName.ShouldBe("some_queue");
        }
    }
}