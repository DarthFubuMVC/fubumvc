using System.Linq;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class ErrorHandlerBehavior_attachment_to_the_handler_chains
    {
        [Test]
        public void should_have_an_error_behavior_on_each_chain()
        {
            var graph = FubuTransport.BehaviorGraphFor(x => { });

            foreach (HandlerChain chain in graph.Handlers)
            {
                chain.First().ShouldBeOfType<ExceptionHandlerNode>()
                     .Chain.ShouldBeTheSameAs(chain);


            }
        }
    }
}