using FubuTransportation.Configuration;
using FubuTransportation.ErrorHandling;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;

namespace FubuTransportation.Testing.ErrorHandling
{
    [TestFixture]
    public class ErrorHandlerBehavior_attachment_to_the_handler_chains
    {
        [Test]
        public void should_have_an_error_behavior_on_each_chain()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(x => { });

            foreach (HandlerChain chain in graph)
            {
                chain.First().ShouldBeOfType<ExceptionHandlerNode>()
                     .Chain.ShouldBeTheSameAs(chain);


            }
        }
    }
}