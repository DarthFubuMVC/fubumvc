using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using NUnit.Framework;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    [TestFixture]
    public class HandlerChainTester
    {

        [Test]
        public void normal_handler_chains_are_not_polling_jobs()
        {
            new HandlerChain().IsPollingJob().ShouldBeFalse();
        }

        [Test]
        public void the_default_number_of_maximum_attempts_is_1()
        {
            new HandlerChain().MaximumAttempts.ShouldBe(1);
        }

        [Test]
        public void HandlerChain_cannot_be_marked_as_partial_only_because_it_knocks_out_diagnostic_tracing()
        {
            new HandlerChain().IsPartialOnly.ShouldBeFalse();
        }


        [Test]
        public void is_async_negative()
        {
            var chain = new HandlerChain();
            chain.IsAsync.ShouldBeFalse();
        
            chain.AddToEnd(HandlerCall.For<GreenHandler>(x => x.Handle(new Message1())));

            chain.IsAsync.ShouldBeFalse();
        
        }

        [Test]
        public void is_async_positive()
        {
            var chain = new HandlerChain();
            chain.IsAsync.ShouldBeFalse();

            chain.AddToEnd(HandlerCall.For<GreenHandler>(x => x.Handle(new Message1())));
            chain.AddToEnd(HandlerCall.For<AsyncHandler>(x => x.Go(null)));

            chain.IsAsync.ShouldBeTrue();


        }

        public class AsyncHandler
        {
            public Task Go(Message message)
            {
                return null;
            }

            public Task<string> Other(Message message)
            {
                return null;
            }
        }
    }
}