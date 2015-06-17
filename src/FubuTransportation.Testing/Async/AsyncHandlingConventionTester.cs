using System.Linq;
using System.Threading.Tasks;
using FubuTestingSupport;
using FubuTransportation.Async;
using FubuTransportation.Configuration;
using FubuTransportation.Registration.Nodes;
using FubuTransportation.Testing.ScenarioSupport;
using NUnit.Framework;

namespace FubuTransportation.Testing.Async
{
    [TestFixture]
    public class AsyncHandlingConventionTester
    {
        [Test]
        public void async_handling_node_should_be_right_before_any_calls()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(x => { });

            graph.ChainFor(typeof (Message)).OfType<HandlerCall>()
                .First().Previous.ShouldBeOfType<AsyncHandlingNode>();

            graph.ChainFor(typeof(Message3)).OfType<HandlerCall>()
                .First().Previous.ShouldBeOfType<AsyncHandlingNode>();
        }
    }

    public class AsyncTaskHandler
    {
        public Task Go(Message message)
        {
            return null;
        }

        public Task<Message2> Other(Message3 message)
        {
            return null;
        }
    }
}