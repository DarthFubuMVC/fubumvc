using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using NUnit.Framework;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Async
{
    [TestFixture]
    public class AsyncHandlingConventionTester
    {
        [Test]
        public void async_handling_node_should_be_right_before_any_calls()
        {
            using (var runtime = FubuRuntime.BasicBus())
            {
                var graph = runtime.Behaviors;

                graph.ChainFor(typeof(Message)).OfType<HandlerCall>()
                    .First().Previous.ShouldBeOfType<AsyncHandlingNode>();

                graph.ChainFor(typeof(Message3)).OfType<HandlerCall>()
                    .First().Previous.ShouldBeOfType<AsyncHandlingNode>();
            }


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