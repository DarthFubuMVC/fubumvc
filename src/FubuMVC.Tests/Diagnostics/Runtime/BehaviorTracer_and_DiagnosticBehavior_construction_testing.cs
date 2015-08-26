using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.StructureMap;
using NUnit.Framework;
using Shouldly;
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



        [Test]
        public void creating_an_object_def_for_full_tracing_should_wrap_with_a_behavior_tracer()
        {
            var node = new Wrapper(typeof (SimpleBehavior));
            var instance = new BehaviorTracerNode(node).As<IContainerModel>().ToInstance().As<IConfiguredInstance>();

            instance.PluggedType.ShouldBe(typeof (BehaviorTracer));
            instance.FindDependencyDefinitionFor<IActionBehavior>()
                .ReturnedType.ShouldBe(typeof (SimpleBehavior));
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

        public class DifferentBehavior : SimpleBehavior
        {
        }


        public class Controller1
        {
            public void Go(Input1 input)
            {
            }

            public class Input1
            {
            }
        }
    }
}