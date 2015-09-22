using System.Linq;
using FubuMVC.Core;
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
            using (var runtime = FubuRuntime.BasicBus())
            {
                foreach (HandlerChain chain in runtime.Behaviors.Handlers)
                {
                    chain.First().ShouldBeOfType<ExceptionHandlerNode>()
                         .Chain.ShouldBeTheSameAs(chain);


                }
            }

        }
    }
}