using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class FubuTransportRegistry_default_handler_source_policies_spec
    {
        [Test]
        public void should_automatically_pick_up_classes_suffixed_with_Handler()
        {
            FubuTransportRegistry.HandlerGraphFor(r => {})
                .ChainFor(typeof(Message1)).OfType<HandlerCall>().Any(x => x.HandlerType == typeof(MyNewHandler))
                .ShouldBeTrue();
        }

        [Test]
        public void should_automatically_pick_up_classes_suffixed_with_Consumer()
        {
            FubuTransportRegistry.HandlerGraphFor(r => { })
                .ChainFor(typeof(Message1)).OfType<HandlerCall>().Any(x => x.HandlerType == typeof(MyNewConsumer))
                .ShouldBeTrue();
        }

        [Test]
        public void default_handler_sources_are_not_used_if_a_custom_one_is_registered_and_disabled()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(r => {
                r.Handlers.DisableDefaultHandlerSource();
                r.Handlers.FindBy<MyFunkyHandlerSource>();
            });

            graph.ChainFor(typeof (Message1)).OfType<HandlerCall>().Select(x => x.HandlerType)
                .Single().ShouldEqual(typeof (MyFunkySpaceAgeProcessor));


        }

        [Test]
        public void default_handler_sources_are_not_used_if_a_custom_one_is_registered_2_and_disabled()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(r =>
            {
                r.Handlers.FindBy(new MyFunkyHandlerSource());
                r.Handlers.DisableDefaultHandlerSource();
            });

            graph.ChainFor(typeof(Message1)).OfType<HandlerCall>().Select(x => x.HandlerType)
                .Single().ShouldEqual(typeof(MyFunkySpaceAgeProcessor));


        }

        [Test]
        public void can_override_the_default_handler_source_by_explicits_and_disabled()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(r =>
            {
                
                r.Handlers.FindBy(x => {
                    x.IncludeClassesSuffixedWithConsumer();
                });

                r.Handlers.DisableDefaultHandlerSource();
            });

            graph.ChainFor(typeof(Message1)).OfType<HandlerCall>().Any(x => x.HandlerType == typeof(MyNewConsumer))
                .ShouldBeTrue();

            graph.ChainFor(typeof(Message1)).OfType<HandlerCall>().Any(x => x.HandlerType == typeof(MyNewHandler))
                .ShouldBeFalse();
        }
    }


    public class MyFunkyHandlerSource : IHandlerSource
    {
        public IEnumerable<HandlerCall> FindCalls(Assembly applicationAssembly)
        {
            yield return HandlerCall.For<MyFunkySpaceAgeProcessor>(x => x.Go(null));
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