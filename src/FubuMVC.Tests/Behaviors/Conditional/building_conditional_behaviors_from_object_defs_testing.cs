using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.Behaviors.Conditional
{
    [TestFixture]
    public class building_conditional_behaviors_from_object_defs_testing
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            WrappedBehaviorWasInvoked = false;
            NextBehaviorWasInvoked = false;

            theContainer = new Container();

            theRequest = new InMemoryFubuRequest();
            theContainer.Inject<IFubuRequest>(theRequest);

            theService = new Service();
            theContainer.Inject(theService);

            theNode = Wrapper.For<MyBehavior>();
            theNode.AddAfter(Wrapper.For<FollowingBehavior>());
        }

        #endregion

        private Container theContainer;
        private InMemoryFubuRequest theRequest;
        private Service theService;
        private static bool WrappedBehaviorWasInvoked;
        private static bool NextBehaviorWasInvoked;
        private Wrapper theNode;

        private IActionBehavior behavior()
        {
            var objectDef = theNode.As<IContainerModel>().ToObjectDef();
            var instance = new ObjectDefInstance(objectDef);

            return theContainer.GetInstance<IActionBehavior>(instance);
        }

        public class MyBehavior : IActionBehavior
        {
            public IActionBehavior InnerBehavior { get; set; }

            public void Invoke()
            {
                WrappedBehaviorWasInvoked = true;
                if (InnerBehavior != null) InnerBehavior.Invoke();
            }

            public void InvokePartial()
            {
                WrappedBehaviorWasInvoked = true;
                if (InnerBehavior != null) InnerBehavior.InvokePartial();
            }
        }

        public class FollowingBehavior : IActionBehavior
        {
            public IActionBehavior InnerBehavior { get; set; }

            public void Invoke()
            {
                NextBehaviorWasInvoked = true;
            }

            public void InvokePartial()
            {
                NextBehaviorWasInvoked = true;
            }
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

        [Test]
        public void create_the_behavior_with_custom_conditional_and_no_diagnostics()
        {
            theNode.Condition<CustomConditional>();
            var behavior = this.behavior();

            var invoker = behavior.ShouldBeOfType<ConditionalBehaviorInvoker>();
            var conditionalBehavior = invoker.ConditionalBehavior.ShouldBeOfType<ConditionalBehavior>();

            conditionalBehavior.InnerBehavior.ShouldBeOfType<MyBehavior>();
            conditionalBehavior.Condition.ShouldBeOfType<CustomConditional>();

            invoker.InnerBehavior.ShouldBeOfType<FollowingBehavior>();
        }

        [Test]
        public void create_the_behavior_without_any_condition_or_diagnostics()
        {
            var actionBehavior = behavior();
            actionBehavior.ShouldBeOfType<MyBehavior>().InnerBehavior.ShouldBeOfType<FollowingBehavior>();

            actionBehavior.Invoke();

            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_custom_conditional_negative_condition()
        {
            CustomConditional.IsTrue = false;
            theNode.Condition<CustomConditional>();
            var behavior = this.behavior();

            behavior.Invoke();


            WrappedBehaviorWasInvoked.ShouldBeFalse();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_custom_conditional_positive_condition()
        {
            CustomConditional.IsTrue = true;
            theNode.Condition<CustomConditional>();
            var behavior = this.behavior();

            behavior.Invoke();


            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }


        [Test]
        public void execute_behavior_with_func_against_model_negative()
        {
            theRequest.Set(new Model{
                IsTrue = false
            });

            theNode.ConditionByModel<Model>(x => x.IsTrue);

            behavior().Invoke();

            WrappedBehaviorWasInvoked.ShouldBeFalse();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_func_against_model_positive()
        {
            theRequest.Set(new Model{
                IsTrue = true
            });

            theNode.ConditionByModel<Model>(x => x.IsTrue);

            behavior().Invoke();

            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }


        [Test]
        public void execute_behavior_with_func_against_service_negative()
        {
            theService.IsTrue = false;

            theNode.ConditionByService<Service>(x => x.IsTrue);

            behavior().Invoke();

            WrappedBehaviorWasInvoked.ShouldBeFalse();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_func_against_service_positive()
        {
            theService.IsTrue = true;

            theNode.ConditionByService<Service>(x => x.IsTrue);

            behavior().Invoke();

            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_straight_func_negative()
        {
            theNode.Condition(() => false);

            var behavior = this.behavior();

            behavior.Invoke();


            WrappedBehaviorWasInvoked.ShouldBeFalse();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }

        [Test]
        public void execute_behavior_with_straight_func_positive()
        {
            theNode.Condition(() => true);

            var behavior = this.behavior();

            behavior.Invoke();


            WrappedBehaviorWasInvoked.ShouldBeTrue();
            NextBehaviorWasInvoked.ShouldBeTrue();
        }
    }
}