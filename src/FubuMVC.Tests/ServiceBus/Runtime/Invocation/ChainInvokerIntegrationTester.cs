using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.Services.Messaging.Tracking;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class ChainInvokerIntegrationTester
    {
        [SetUp]
        public void SetUp()
        {
            InMemoryQueueManager.ClearAll();
        }

        [Test]
        public void invoking_a_chain_will_execute_completely_with_cascading_messages()
        {
            FubuTransport.SetupForInMemoryTesting();
            using (var runtime = FubuApplication.BootstrapApplication<ChainInvokerApplication>())
            {
                var recorder = runtime.Factory.Get<MessageRecorder>();

                var invoker = runtime.Factory.Get<IChainInvoker>();

                MessageHistory.WaitForWorkToFinish(() =>
                {
                    invoker.InvokeNow(new WebMessage { Text = "I'm good" });
                });

                recorder.Messages.Each(x => Debug.WriteLine(x));

                // Should process all the cascading messages that bubble up
                // and their cascaded messages
                recorder.Messages.ShouldContain("I'm good");
                recorder.Messages.ShouldContain("I'm good-2");
                recorder.Messages.ShouldContain("I'm good-2-4");
                recorder.Messages.ShouldContain("I'm good-2-3");
                recorder.Messages.ShouldContain("Traced: I'm good");
            }
        }

        [Test]
        public void invoking_a_chain_will_execute_with_failure_does_not_send_off_cascading_messages()
        {
            FubuTransport.SetupForInMemoryTesting();
            using (var runtime = FubuApplication.BootstrapApplication<ChainInvokerApplication>())
            {
                var recorder = runtime.Factory.Get<MessageRecorder>();

                var invoker = runtime.Factory.Get<IChainInvoker>();


                MessageHistory.WaitForWorkToFinish(() =>
                {
                    // The handler for WebMessage is rigged to throw exceptions
                    // if it contains the text 'Bad'
                    invoker.InvokeNow(new WebMessage { Text = "Bad message" });
                });

                recorder.Messages.Each(x => Debug.WriteLine(x));

                // NO MESSAGES SHOULD GET OUT WITH THE ORIGINAL 'Bad Message'
                recorder.Messages.Any(x => x.Contains("Bad message")).ShouldBeFalse();

                AssertCascadedMessages(recorder);
            }
        }

        [Test]
        public void invoking_a_chain_will_execute_completely_with_cascading_immediate_continuations()
        {
            FubuTransport.SetupForInMemoryTesting();
            using (var runtime = FubuApplication.BootstrapApplication<ChainInvokerApplication>())
            {
                var recorder = runtime.Factory.Get<MessageRecorder>();

                var invoker = runtime.Factory.Get<IChainInvoker>();

                MessageHistory.WaitForWorkToFinish(() =>
                {
                    invoker.InvokeNow(new TriggerImmediate { Text = "First", ContinueText = "I'm good" });
                });

                recorder.Messages.Each(x => Debug.WriteLine(x));

                // Should process all the cascading messages that bubble up
                // and their cascaded messages
                recorder.Messages.ShouldContain("First");
                recorder.Messages.ShouldContain("I'm good");
                recorder.Messages.ShouldContain("I'm good-2");
                recorder.Messages.ShouldContain("I'm good-2-4");
                recorder.Messages.ShouldContain("I'm good-2-3");
                recorder.Messages.ShouldContain("Traced: I'm good");
            }
        }

        [Test]
        public void invoking_a_chain_will_execute_completely_with_cascading_immediate_continuations_even_if_the_continuation_messages_fail()
        {
            FubuTransport.SetupForInMemoryTesting();
            using (var runtime = FubuApplication.BootstrapApplication<ChainInvokerApplication>())
            {
                var recorder = runtime.Factory.Get<MessageRecorder>();

                var invoker = runtime.Factory.Get<IChainInvoker>();

                MessageHistory.WaitForWorkToFinish(() =>
                {
                    invoker.InvokeNow(new TriggerImmediate { Text = "First", ContinueText = "Bad message" });
                });

                recorder.Messages.Each(x => Debug.WriteLine(x));

                // Should process all the cascading messages that bubble up
                // and their cascaded messages
                recorder.Messages.ShouldContain("First");

                AssertCascadedMessages(recorder);
            }
        }

        [Test]
        public void invoking_a_chain_will_execute_completely_with_cascading_immediate_continuations_even_if_the_continuation_messages_fail_and_retry_immediately()
        {
            FubuTransport.SetupForInMemoryTesting();
            using (var runtime = FubuApplication.BootstrapApplication<ChainInvokerApplication>())
            {
                var recorder = runtime.Factory.Get<MessageRecorder>();

                var invoker = runtime.Factory.Get<IChainInvoker>();

                MessageHistory.WaitForWorkToFinish(() =>
                {
                    invoker.InvokeNow(new TriggerImmediate { Text = "First", ContinueText = "Retry message" });
                });

                recorder.Messages.Each(x => Debug.WriteLine(x));

                // Should process all the cascading messages that bubble up
                // and their cascaded messages
                recorder.Messages.ShouldContain("First");

                AssertCascadedMessages(recorder);
            }
        }

        private static void AssertCascadedMessages(MessageRecorder recorder)
        {
            // will succeed on the retry because we change the text in the handler.
            // basically just proving that the interplay w/ exception handling behaviors
            // and continuations within the invocation is working

            recorder.Messages.ShouldContain("now it is good");
            recorder.Messages.ShouldContain("now it is good-2");
            recorder.Messages.ShouldContain("now it is good-2-4");
            recorder.Messages.ShouldContain("now it is good-2-3");
            recorder.Messages.ShouldContain("Traced: now it is good");
        }
    }


    public class ChainInvokerSettings
    {
        public Uri Incoming { get; set; }
    }

    public class ChainInvokerApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<ChainInvokerTransportRegistry>();
        }
    }

    public class ChainInvokerTransportRegistry : FubuTransportRegistry<ChainInvokerSettings>
    {
        public ChainInvokerTransportRegistry()
        {
            Services.ForSingletonOf<MessageRecorder>();

            AlterSettings<TransportSettings>(x =>
            {
                x.EnableInMemoryTransport = true;
                x.DebugEnabled = true;
            });

            Policies.Global.Add<ErrorHandlingPolicy>();

            Channel(x => x.Incoming).AcceptsMessages(t => true).ReadIncoming();
        }
    }

    public class ErrorHandlingPolicy : HandlerChainPolicy
    {
        public override void Configure(HandlerChain chain)
        {
            chain.MaximumAttempts = 3;
            chain.OnException<DivideByZeroException>().Requeue();
            chain.OnException<InvalidOperationException>().Retry();
        }
    }
}