using System.Linq;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Tests.ServiceBus;
using Shouldly;
using NUnit.Framework;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class FubuTransportRegistry_HandlerSource_registration_Tester
    {
        [Test]
        public void can_register_a_handler_source_by_explicit_config()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(x => {
                x.Handlers.DisableDefaultHandlerSource();
                x.Handlers.FindBy(source => {
                    source.UseThisAssembly();
                    source.IncludeTypesNamed(name => name.Contains("FooHandler"));
                });
            });

            graph.SelectMany(x => x.OfType<HandlerCall>()).Where(x => x.HandlerType.Assembly != typeof(HandlerCall).Assembly).Select(x => x.HandlerType).OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(MyFooHandler), typeof(MyOtherFooHandler));

        }

        [Test]
        public void extra_handler_sources_are_additive()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(x =>
            {
                x.Handlers.Include<RandomThing>();
            });

            var handlerTypes = graph.SelectMany(x => x.OfType<HandlerCall>()).Select(x => x.HandlerType).ToArray();
            handlerTypes.ShouldContain(typeof(MyOtherConsumer));
            handlerTypes.ShouldContain(typeof(MyFooHandler));
            handlerTypes.ShouldContain(typeof(RandomThing));

        }
    }

    public class RandomThing
    {
        public void Consume(Message1 message1){}
    }

    public class MyFooHandler
    {
        public void Handle(Message1 message1){}
    }

    public class MyOtherFooHandler
    {
        public void Handle(Message2 message2){}
    }

    public class MissingCompletedSaga
    {
        public SuccessSagaState State { get; set; }
    }

    public class MissingStateSaga
    {
        public bool IsCompleted { get; set; }
    }

    public class SuccessMatchesTestSaga
    {
        public bool IsCompleted { get; set; }
        public SuccessSagaState State { get; set; }
    }

    public class NoSagaSuffix
    {
        public bool IsCompleted { get; set; }
        public SuccessSagaState State { get; set; }
    }

    public class SuccessSagaState
    {
    }
}