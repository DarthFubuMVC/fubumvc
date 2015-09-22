using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class FubuTransportRegistry_default_handler_source_policies_spec
    {
        [Test]
        public void should_automatically_pick_up_classes_suffixed_with_Handler()
        {
            using (var runtime = FubuRuntime.BasicBus())
            {
                runtime.Behaviors.ChainFor(typeof(Message1)).OfType<HandlerCall>().Any(x => x.HandlerType == typeof(MyNewHandler))
                .ShouldBeTrue();
            }

        }

        [Test]
        public void should_automatically_pick_up_classes_suffixed_with_Consumer()
        {
            using (var runtime = FubuRuntime.BasicBus())
            {
                runtime.Behaviors.ChainFor(typeof(Message1)).OfType<HandlerCall>().Any(x => x.HandlerType == typeof(MyNewConsumer))
                .ShouldBeTrue();
            }
        }

        [Test]
        public void default_handler_sources_are_not_used_if_a_custom_one_is_registered_and_disabled()
        {
            using (var runtime = FubuRuntime.BasicBus(r =>
            {
                r.Handlers.DisableDefaultHandlerSource();
                r.Handlers.FindBy<MyFunkyHandlerSource>();
            }))
            {
                runtime.Behaviors.ChainFor(typeof (Message1)).OfType<HandlerCall>().Select(x => x.HandlerType)
                .Single().ShouldBe(typeof (MyFunkySpaceAgeProcessor));
            }

        }

        [Test]
        public void default_handler_sources_are_not_used_if_a_custom_one_is_registered_2_and_disabled()
        {
            using (var runtime = FubuRuntime.BasicBus(r =>
            {
                r.Handlers.FindBy(new MyFunkyHandlerSource());
                r.Handlers.DisableDefaultHandlerSource();
            }))
            {
                runtime.Behaviors.ChainFor(typeof(Message1)).OfType<HandlerCall>().Select(x => x.HandlerType)
                .Single().ShouldBe(typeof(MyFunkySpaceAgeProcessor));
            }


        }

        [Test]
        public void can_override_the_default_handler_source_by_explicits_and_disabled()
        {
            using (var runtime = FubuRuntime.BasicBus(r =>
            {
                r.Handlers.FindBy(x => x.IncludeClassesSuffixedWithConsumer());

                r.Handlers.DisableDefaultHandlerSource();
            }))
            {
                var graph = runtime.Behaviors;
                graph.ChainFor(typeof(Message1)).OfType<HandlerCall>().Any(x => x.HandlerType == typeof(MyNewConsumer))
                    .ShouldBeTrue();

                graph.ChainFor(typeof(Message1)).OfType<HandlerCall>().Any(x => x.HandlerType == typeof(MyNewHandler))
                    .ShouldBeFalse();
            }


        }
    }


    public class MyFunkyHandlerSource : IHandlerSource
    {
        public Task<HandlerCall[]> FindCalls(Assembly applicationAssembly)
        {
            return Task.Factory.StartNew(() => new [] { HandlerCall.For<MyFunkySpaceAgeProcessor>(x => x.Go(null)) });

            
        }
    }

    public class MyFunkySpaceAgeProcessor
    {
        public void Go(Message1 message){}
    }

    public class MyNewHandler
    {
        public void Go(Message1 input){}
    }

    public class MyNewConsumer
    {
        public void Go(Message1 input){}
    }
}