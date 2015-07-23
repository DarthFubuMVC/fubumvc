using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    [TestFixture]
    public class BehaviorTracer_and_DiagnosticBehavior_construction_testing
    {
        private BehaviorChain theChain;
        private Guid theOriginalGuid;

        [SetUp]
        public void SetUp()
        {
            theChain = new BehaviorChain();
            var action = ActionCall.For<Controller1>(x => x.Go(null));
            theChain.AddToEnd(action);

            theOriginalGuid = action.UniqueId;
        }

        private IConfiguredInstance toInstance()
        {
            return (IConfiguredInstance) theChain.As<IContainerModel>().ToInstance();
        }

        [Test]
        public void when_in_diagnostic_mode_use_a_diagnostic_behavior_wrapping_the_whole_with_the_same_name()
        {
            new BehaviorTracerNode(theChain.Top);
            new DiagnosticNode(theChain);

            var instance = toInstance();
            instance.PluggedType.ShouldEqual(typeof(DiagnosticBehavior));


            instance.FindDependencyDefinitionFor<IActionBehavior>()
                .ReturnedType.ShouldEqual(typeof(BehaviorTracer));

            instance
                .FindDependencyDefinitionFor<IActionBehavior>().As<IConfiguredInstance>()
                .FindDependencyDefinitionFor<IActionBehavior>()
                .ReturnedType.ShouldEqual(typeof(OneInZeroOutActionInvoker<Controller1, Controller1.Input1>));

        }


        [Test]
        public void behavior_tracers_deeper()
        {
            var node = Wrapper.For<SimpleBehavior>();
            var chain = new RoutedChain("foo");
            chain.AddToEnd(node);
            node.AddAfter(Wrapper.For<DifferentBehavior>());

            ApplyTracing.ApplyToChain(chain);
            
            

            var instance = chain.As<IContainerModel>().ToInstance()
                .As<IConfiguredInstance>()
                .FindDependencyDefinitionFor<IActionBehavior>()
                .As<IConfiguredInstance>();

            instance.PluggedType.ShouldEqual(typeof(BehaviorTracer));
            var child1 = instance.FindDependencyDefinitionFor<IActionBehavior>().As<IConfiguredInstance>();
            child1.PluggedType.ShouldEqual(typeof(SimpleBehavior));

            var child2 = child1.FindDependencyDefinitionFor<IActionBehavior>().As<IConfiguredInstance>();
            child2.PluggedType.ShouldEqual(typeof(BehaviorTracer));

            var child3 = child2.FindDependencyDefinitionFor<IActionBehavior>();
            child3.ReturnedType.ShouldEqual(typeof(DifferentBehavior));
        }



        [Test]
        public void creating_an_object_def_for_full_tracing_should_wrap_with_a_behavior_tracer()
        {
            var node = new Wrapper(typeof(SimpleBehavior));
            var instance = new BehaviorTracerNode(node).As<IContainerModel>().ToInstance().As<IConfiguredInstance>();

            instance.PluggedType.ShouldEqual(typeof(BehaviorTracer));
            instance.FindDependencyDefinitionFor<IActionBehavior>()
                .ReturnedType.ShouldEqual(typeof(SimpleBehavior));
        }




        public class SimpleBehavior : IActionBehavior
        {
            public void Invoke()
            {

            }

            public void InvokePartial()
            {
            }
        }

        public class DifferentBehavior : SimpleBehavior { }
    


        public class Controller1
        {
            public void Go(Input1 input) { }

            public class Input1 { }
        }


    }
}