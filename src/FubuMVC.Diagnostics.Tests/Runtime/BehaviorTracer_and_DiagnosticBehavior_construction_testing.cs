using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
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

        private ObjectDef toObjectDef()
        {
            return theChain.As<IContainerModel>().ToObjectDef();
        }

        [Test]
        public void when_in_diagnostic_mode_use_a_diagnostic_behavior_wrapping_the_whole_with_the_same_name()
        {
            new BehaviorTracerNode(theChain.Top);
            new DiagnosticNode(theChain);

            var objectDef = toObjectDef();
            objectDef.Type.ShouldEqual(typeof(DiagnosticBehavior));


            objectDef.FindDependencyDefinitionFor<IActionBehavior>()
                .Type.ShouldEqual(typeof(BehaviorTracer));

            objectDef
                .FindDependencyDefinitionFor<IActionBehavior>()
                .FindDependencyDefinitionFor<IActionBehavior>()
                .Type.ShouldEqual(typeof(OneInZeroOutActionInvoker<Controller1, Controller1.Input1>));

        }


        [Test]
        public void behavior_tracers_deeper()
        {
            var node = Wrapper.For<SimpleBehavior>();
            var chain = new RoutedChain("foo");
            chain.AddToEnd(node);
            node.AddAfter(Wrapper.For<DifferentBehavior>());

            ApplyTracing.ApplyToChain(chain);
            
            

            var objectDef = chain.As<IContainerModel>().ToObjectDef().FindDependencyDefinitionFor<IActionBehavior>();

            objectDef.Type.ShouldEqual(typeof(BehaviorTracer));
            var child1 = objectDef.FindDependencyDefinitionFor<IActionBehavior>();
            child1.Type.ShouldEqual(typeof(SimpleBehavior));

            var child2 = child1.FindDependencyDefinitionFor<IActionBehavior>();
            child2.Type.ShouldEqual(typeof(BehaviorTracer));

            var child3 = child2.FindDependencyDefinitionFor<IActionBehavior>();
            child3.Type.ShouldEqual(typeof(DifferentBehavior));
        }



        [Test]
        public void creating_an_object_def_for_full_tracing_should_wrap_with_a_behavior_tracer()
        {
            var node = new Wrapper(typeof(SimpleBehavior));
            var objectDef = new BehaviorTracerNode(node).As<IContainerModel>().ToObjectDef();

            objectDef.Type.ShouldEqual(typeof(BehaviorTracer));
            objectDef.DependencyFor<IActionBehavior>().As<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof(SimpleBehavior));
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