using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class FubuTransportRegistry_HandlerSource_registration_Tester
    {
        [Test]
        public void can_register_a_handler_source_by_explicit_config()
        {
            using (var runtime = FubuRuntime.BasicBus(x =>
            {
                x.Handlers.DisableDefaultHandlerSource();
                x.Handlers.FindBy(source =>
                {
                    source.UseThisAssembly();
                    source.IncludeTypesNamed(name => name.Contains("FooHandler"));
                });
            }))
            {
                var graph = runtime.Behaviors;


                var handlers =
                    graph.Chains.SelectMany(x => x.OfType<HandlerCall>())
                        .Where(x => x.HandlerType.Assembly != typeof (HandlerCall).Assembly)
                        .Select(x => x.HandlerType)
                        .ToArray();


                handlers.ShouldContain(typeof (MyFooHandler));
                handlers.ShouldContain(typeof (MyOtherFooHandler));
            }
        }

        [Test]
        public void extra_handler_sources_are_additive()
        {
            using (var runtime = FubuRuntime.BasicBus(x => { x.Handlers.Include<RandomThing>(); }))
            {
                var graph = runtime.Behaviors;

                var handlerTypes =
                    graph.Chains.SelectMany(x => x.OfType<HandlerCall>()).Select(x => x.HandlerType).ToArray();
                handlerTypes.ShouldContain(typeof (MyOtherConsumer));
                handlerTypes.ShouldContain(typeof (MyFooHandler));
                handlerTypes.ShouldContain(typeof (RandomThing));
            }
        }
    }

    public class RandomThing
    {
        public void Consume(Message1 message1)
        {
        }
    }

    public class MyFooHandler
    {
        public void Handle(Message1 message1)
        {
        }
    }

    public class MyOtherFooHandler
    {
        public void Handle(Message2 message2)
        {
        }
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