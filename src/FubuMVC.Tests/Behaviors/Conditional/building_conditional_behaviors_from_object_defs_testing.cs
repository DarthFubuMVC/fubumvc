using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.StructureMap;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.Tests.Behaviors.Conditional
{
    [TestFixture]
    public class building_conditional_behaviors_from_object_defs_testing
    {
        private Container theContainer;
        private InMemoryFubuRequest theRequest;
        private Service theService;
        private static bool WrappedBehaviorWasInvoked;
        private static bool NextBehaviorWasInvoked;
        private Wrapper theNode;

        [SetUp]
        public void SetUp()
        {
            WrappedBehaviorWasInvoked = false;
            NextBehaviorWasInvoked = false;

            theContainer = new Container();
            theContainer.Inject<IDebugReport>(new DebugReport(null, null));
            theContainer.Inject(MockRepository.GenerateMock<IDebugDetector>());

            theRequest = new InMemoryFubuRequest();
            theContainer.Inject<IFubuRequest>(theRequest);

            theService = new Service();
            theContainer.Inject(theService);

            theNode = Wrapper.For<MyBehavior>();
            theNode.AddAfter(Wrapper.For<FollowingBehavior>());
        }

        private IActionBehavior behavior(DiagnosticLevel level)
        {
            var objectDef = theNode.As<IContainerModel>().ToObjectDef(level);
            var instance = new ObjectDefInstance(objectDef);

            return theContainer.GetInstance<IActionBehavior>(instance);
        }

        public class MyBehavior : IActionBehavior
        {
            public IActionBehavior InnerBehavior { get; set; }

            public void Invoke()
            {
                building_conditional_behaviors_from_object_defs_testing.WrappedBehaviorWasInvoked = true;
                if (InnerBehavior != null) InnerBehavior.Invoke();
            }

            public void InvokePartial()
            {
                building_conditional_behaviors_from_object_defs_testing.WrappedBehaviorWasInvoked = true;
                if (InnerBehavior != null) InnerBehavior.InvokePartial();
            }
        }

        public class FollowingBehavior : IActionBehavior
        {
            public IActionBehavior InnerBehavior { get; set; }

            public void Invoke()
            {
                building_conditional_behaviors_from_object_defs_testing.NextBehaviorWasInvoked = true;
            }

            public void InvokePartial()
            {
                building_conditional_behaviors_from_object_defs_testing.NextBehaviorWasInvoked = true;
            }
        }

        [Test]
        public void create_the_behavior_without_any_condition_or_diagnostics()
        {
            var actionBehavior = behavior(DiagnosticLevel.None);
            actionBehavior.ShouldBeOfType<MyBehavior>().InnerBehavior.ShouldBeOfType<FollowingBehavior>();

            actionBehavior.Invoke();

            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void create_the_behavior_with_custom_conditional_and_no_diagnostics()
        {
            theNode.Condition<CustomConditional>();
            var behavior = this.behavior(DiagnosticLevel.None);

            var invoker= behavior.ShouldBeOfType<ConditionalBehaviorInvoker>();
            var conditionalBehavior = invoker.ConditionalBehavior.ShouldBeOfType<ConditionalBehavior>();

            conditionalBehavior.InnerBehavior.ShouldBeOfType<MyBehavior>();
            conditionalBehavior.Condition.ShouldBeOfType<CustomConditional>();

            invoker.InnerBehavior.ShouldBeOfType<FollowingBehavior>();
        }

        [Test]
        public void create_the_behavior_with_custom_conditional_and_diagnostics()
        {
            theNode.Condition<CustomConditional>();
            var behavior = this.behavior(DiagnosticLevel.FullRequestTracing).ShouldBeOfType<BehaviorTracer>()
                .Inner;

            var invoker = behavior.ShouldBeOfType<ConditionalBehaviorInvoker>();
            var conditionalBehavior = invoker.ConditionalBehavior.ShouldBeOfType<ConditionalBehavior>();

            conditionalBehavior.InnerBehavior.ShouldBeOfType<MyBehavior>();
            conditionalBehavior.Condition.ShouldBeOfType<CustomConditional>();

            invoker.InnerBehavior.ShouldBeOfType<BehaviorTracer>()
                .Inner.ShouldBeOfType<FollowingBehavior>();
        }

        [Test]
        public void execute_behavior_with_custom_conditional_positive_condition()
        {
            CustomConditional.IsTrue = true;
            theNode.Condition<CustomConditional>();
            var behavior = this.behavior(DiagnosticLevel.None);

            behavior.Invoke();


            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_custom_conditional_negative_condition()
        {
            CustomConditional.IsTrue = false;
            theNode.Condition<CustomConditional>();
            var behavior = this.behavior(DiagnosticLevel.None);

            behavior.Invoke();


            WrappedBehaviorWasInvoked.ShouldBeFalse();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_straight_func_positive()
        {
            theNode.Condition(() => true);

            var behavior = this.behavior(DiagnosticLevel.None);

            behavior.Invoke();


            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_straight_func_negative()
        {
            theNode.Condition(() => false);

            var behavior = this.behavior(DiagnosticLevel.None);

            behavior.Invoke();


            WrappedBehaviorWasInvoked.ShouldBeFalse();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_func_against_model_positive()
        {
            theRequest.Set(new Model(){
                IsTrue = true
            });

            theNode.ConditionByModel<Model>(x => x.IsTrue);

            behavior(DiagnosticLevel.None).Invoke();

            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }


        [Test]
        public void execute_behavior_with_func_against_model_negative()
        {
            theRequest.Set(new Model()
            {
                IsTrue = false
            });

            theNode.ConditionByModel<Model>(x => x.IsTrue);

            behavior(DiagnosticLevel.None).Invoke();

            WrappedBehaviorWasInvoked.ShouldBeFalse();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_func_against_service_positive()
        {
            theService.IsTrue = true;

            theNode.ConditionByService<Service>(x => x.IsTrue);

            behavior(DiagnosticLevel.None).Invoke();

            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }


        [Test]
        public void execute_behavior_with_func_against_service_negative()
        {
            theService.IsTrue = false;

            theNode.ConditionByService<Service>(x => x.IsTrue);

            behavior(DiagnosticLevel.None).Invoke();

            WrappedBehaviorWasInvoked.ShouldBeFalse();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }



        public class CustomConditional : IConditional
        {
            public static bool IsTrue;

            public bool ShouldExecute()
            {
                return IsTrue;
            }
        }

        public class Service
        {
            public bool IsTrue;
        }

        public class Model
        {
            public bool IsTrue;
        }
    }
}