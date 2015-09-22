using System;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Web;
using FubuMVC.LightningQueues;
using FubuMVC.Tests.ServiceBus;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.IntegrationTesting.ServiceBus.Publishing
{
    [TestFixture]
    public class PublishingConfigurationIntegrationTester
    {
        private BehaviorGraph theGraph;
        private BehaviorChain chain;
        private FubuRuntime theRuntime;
        private Container container;
        private IServiceBus theServiceBus;

        [SetUp]
        public void SetUp()
        {
            container = new Container();
            container.Inject(new TransportSettings
            {
                DelayMessagePolling = Int32.MaxValue,
                ListenerCleanupPolling = Int32.MaxValue
            });
            theServiceBus = MockRepository.GenerateMock<IServiceBus>();

            var registry = new FubuRegistry();
            registry.ServiceBus.Enable(true);
            registry.Actions.IncludeType<MessageOnePublisher>();
            registry.StructureMap(container);
            registry.AlterSettings<LightningQueueSettings>(x => x.DisableIfNoChannels = true);

            theRuntime = registry.ToRuntime();
            theGraph = theRuntime.Get<BehaviorGraph>();
            chain = theGraph.ChainFor<MessageOnePublisher>(x => x.post_message1(null));

            container.Inject(theServiceBus);
        
        }

        [TearDown]
        public void Teardown()
        {
            theRuntime.Dispose();
        }

        [Test]
        public void end_to_end_test()
        {
            theRuntime.Scenario(_ =>
            {
                _.Post.Json(new Message1Input());
                _.StatusCodeShouldBeOk();
                _.ContentShouldContain("\"success\":true");
            });

            theServiceBus.AssertWasCalled(x => x.Send(new Message1()), x => x.IgnoreArguments());
        }

        [Test]
        public void should_find_the_IEventPublishers_loaded_into_memory()
        {
            chain.ShouldNotBeNull();
            theGraph.ChainFor<MessageTwoPublisher>(x => x.post_message2(null))
                .ShouldNotBeNull();
        }

        [Test]
        public void input_type_should_be_the_input_type_of_the_publisher_method()
        {
            chain.InputType().ShouldBe(typeof(Message1Input));
        }

        [Test]
        public void resource_type_should_be_AjaxContinuation()
        {
            chain.ResourceType().ShouldBe(typeof(AjaxContinuation));
        }

        [Test]
        public void should_be_a_PublishEvent_node_directly_after_the_publishing_action()
        {
            // diagnostics are in here now.
            chain.FirstCall().Next.ShouldBeOfType<SendsMessage>()
                .EventType.ShouldBe(typeof(Message1));
        }

    }

    public class MessageOnePublisher : ISendMessages
    {
        public Message1 post_message1(Message1Input input)
        {
            return new Message1();
        }

        
    }

    public class MessageTwoPublisher : ISendMessages
    {
        public Message2 post_message2(Message2Input input)
        {
            return new Message2();
        }
    }

    public class Message1Input{}
    public class Message2Input{}
}