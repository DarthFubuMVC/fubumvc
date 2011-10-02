using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class Behavior_node_ObjectDef_creation_Tester
    {
        [Test]
        public void creating_an_object_def_for_no_tracing()
        {
            var node = new Wrapper(typeof (SimpleBehavior));
            node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                .Type.ShouldEqual(typeof (SimpleBehavior));


        }

        [Test]
        public void creating_an_object_def_for_full_tracing_should_wrap_with_a_behavior_tracer()
        {
            var node = new Wrapper(typeof (SimpleBehavior));
            var objectDef = node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.FullRequestTracing);

            objectDef.Type.ShouldEqual(typeof (BehaviorTracer));
            objectDef.DependencyFor<IActionBehavior>().As<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof (SimpleBehavior));
        }

        [Test]
        public void behavior_tracers_deeper()
        {
            var node = Wrapper.For<SimpleBehavior>();
            node.AddAfter(Wrapper.For<DifferentBehavior>());

            var objectDef = node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.FullRequestTracing);

            objectDef.Type.ShouldEqual(typeof(BehaviorTracer));
            var child1 = objectDef.FindDependencyDefinitionFor<IActionBehavior>();
            child1.Type.ShouldEqual(typeof (SimpleBehavior));

            var child2 = child1.FindDependencyDefinitionFor<IActionBehavior>();
            child2.Type.ShouldEqual(typeof (BehaviorTracer));

            var child3 = child2.FindDependencyDefinitionFor<IActionBehavior>();
            child3.Type.ShouldEqual(typeof (DifferentBehavior));
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

        public class DifferentBehavior : SimpleBehavior{}
    }
}