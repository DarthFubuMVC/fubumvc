using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Tests.Registration.Conventions;
using Shouldly;
using Xunit;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Registration
{
    
    public class ActionCallTester
    {

        private ActionCall action = ActionCall.For<ControllerTarget>(x => x.OneInZeroOut(null));

        [Fact]
        public void append_to_descendent_when_next_is_not_null()
        {
            action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            var wrapper = new Wrapper(typeof (FakeBehavior));
            action.AddAfter(wrapper);

            var next = new OutputNode(typeof (Model2));

            action.AddToEnd(next);

            action.Next.ShouldBeTheSameAs(wrapper);
            wrapper.Next.ShouldBeTheSameAs(next);

            action.ShouldHaveTheSameElementsAs(wrapper, next);
        }

        [Fact]
        public void append_when_next_is_null()
        {
            action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            var next = new OutputNode(typeof (Model2));

            action.AddToEnd(next);

            action.Next.ShouldBeTheSameAs(next);
        }

        [Fact]
        public void can_get_the_behavior_type()
        {
            ActionCall.For<ControllerTarget>(c => c.OneInOneOut(null))
                .BehaviorType.ShouldBe(typeof (OneInOneOutActionInvoker<ControllerTarget, Model1, Model2>));
        }

        [Fact]
        public void enrich_puts_the_new_chain_node_directly_behind_the_call()
        {
            action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            var next = new OutputNode(typeof (Model2));


            action.AddToEnd(next);


            var enricher = new Wrapper(typeof (StubBehavior));
            action.AddAfter(enricher);

            action.Next.ShouldBeTheSameAs(enricher);
            enricher.Next.ShouldBeTheSameAs(next);
        }

        public class StubBehavior : IActionBehavior
        {
            public void Invoke()
            {
                throw new NotImplementedException();
            }

            public void InvokePartial()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void fail_to_build_an_action_by_type_for_a_type_with_more_than_one_method()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                () => { ActionCall.For(typeof (InvalidActionWithMultipleMethods)); });
        }

        [Fact]
        public void returns_T_should_tell_if_action_has_output_of_type()
        {
            action = ActionCall.For<ControllerTarget>(c => c.OneInOneOut(null));
            action.Returns<Model2>().ShouldBeTrue();
            action.Returns<object>().ShouldBeTrue();
            action.Returns<Model1>().ShouldBeFalse();
        }

        [Fact]
        public void should_return_if_equal()
        {
            action.Equals(action).ShouldBeTrue();
            action.Equals(null).ShouldBeFalse();
            action.Equals((object) null).ShouldBeFalse();
            action.Equals("").ShouldBeFalse();
        }

        [Fact]
        public void should_return_is_internal_fubu_action()
        {
            action.IsInternalFubuAction().ShouldBeFalse();
        }

        [Fact]
        public void successfully_build_an_action_from_a_handler_type()
        {
            var action = ActionCall.For(typeof (ValidActionWithOneMethod));
            action.HandlerType.ShouldBe(typeof (ValidActionWithOneMethod));
            action.Method.ShouldBe(typeof (ValidActionWithOneMethod).GetMethod("Go"));
        }

        [Fact]
        public void to_definition_with_an_input_type()
        {
            ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null))
                .ToRouteDefinition()
                .Input.ShouldBeOfType<RouteInput<Model1>>();
        }

        [Fact]
        public void to_definition_with_no_input_type()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut());
            action.ToRouteDefinition().ShouldBeOfType<RouteDefinition>().Input.ShouldBeNull();
        }

        [Fact]
        public void to_object_def_throws_when_has_no_return_and_no_input()
        {
            action = ActionCall.For<ControllerTarget>(x => x.ZeroInZeroOut());
            Exception<FubuException>.ShouldBeThrownBy(
                () => action.As<IContainerModel>().ToInstance())
                .ErrorCode.ShouldBe(1005);
        }

        [Fact]
        public void to_object_def_throws_when_has_task_with_no_result_and_no_input()
        {
            action = ActionCall.For<ControllerTarget>(x => x.ZeroInTaskNoResultOut());
            Exception<FubuException>.ShouldBeThrownBy(
                () => action.As<IContainerModel>().ToInstance())
                .ErrorCode.ShouldBe(1005);
        }
    }

    
    public class ActionCallValidationTester
    {
        [Fact]
        public void add_before_must_be_idempotent()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));
            var newNode = new InputNode(typeof (Model1));

            action.AddBefore(newNode);

            action.PreviousNodes.Count().ShouldBe(1);

            action.AddBefore(newNode);

            action.PreviousNodes.Count().ShouldBe(1);
        }

        [Fact]
        public void do_not_append_a_duplicate_node()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));

            // first one is ok
            var newNode = new InputNode(typeof (InputModel));
            action.AddToEnd(newNode);
            action.Count().ShouldBe(1);

            // try it again, the second should be ignored
            action.AddToEnd(newNode);
            action.Count().ShouldBe(1);
        }


        [Fact]
        public void do_not_append_a_duplicate_node_2()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));

            // first one is ok
            var newNode = new InputNode(typeof (InputModel));
            action.AddToEnd(newNode);
            action.Count().ShouldBe(1);

            action.AddToEnd(new Wrapper(typeof (Wrapper1)));

            // try it again, the second should be ignored
            action.AddToEnd(newNode);
            action.Count().ShouldBe(2);
            action.Count(x => x is InputNode).ShouldBe(1);
        }

        [Fact]
        public void should_not_throw_if_call_is_OMIOMO()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            action.Validate();
        }

        [Fact]
        public void should_not_throw_if_call_is_OMIZMO()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.OneInZeroOut(null));
            action.Validate();
        }

        [Fact]
        public void should_not_throw_if_call_is_ZMIOMO()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut());
            action.Validate();
        }

        [Fact]
        public void should_throw_if_input_type_is_value_type()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusOneInput(9));
            var ex = Exception<FubuException>.ShouldBeThrownBy(action.Validate).ShouldBeOfType<FubuException>();
            ex.ErrorCode.ShouldBe(1006);
        }

        [Fact]
        public void should_throw_if_more_than_one_input_parameter()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));
            var ex = Exception<FubuException>.ShouldBeThrownBy(action.Validate).ShouldBeOfType<FubuException>();
            ex.ErrorCode.ShouldBe(1005);
        }

        [Fact]
        public void should_throw_if_return_type_is_value_type()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusReturn());
            var ex = Exception<FubuException>.ShouldBeThrownBy(action.Validate).ShouldBeOfType<FubuException>();
            ex.ErrorCode.ShouldBe(1004);
        }
    }

    public class FakeNode : BehaviorNode
    {
        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }

        protected override IConfiguredInstance buildInstance()
        {
            throw new NotImplementedException();
        }


        public bool Equals(FakeNode other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FakeNode)) return false;
            return Equals((FakeNode) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class FakeBehavior : IActionBehavior
    {
        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }
    }


    public class ValidActionWithOneMethod
    {
        public void Go()
        {
        }
    }

    public class InvalidActionWithMultipleMethods
    {
        public void Go()
        {
        }

        public void Go2()
        {
        }
    }

    public class Model1
    {
        public string Name { get; set; }
    }

    public class Model2
    {
        public string Name { get; set; }
    }

    public class ControllerTarget
    {
        public string LastNameEntered;

        public void ZeroInZeroOut()
        {
        }

        public Task ZeroInTaskNoResultOut()
        {
            return new Task(() => { });
        }

        public Task<Model2> OneInTaskWithOutputOut(Model1 input)
        {
            return new Task<Model2>(() => new Model2
            {
                Name = input.Name
            });
        }

        public Task<Model2> ZeroInTaskWithOutputOut()
        {
            return new Task<Model2>(() => new Model2
            {
                Name = "ZeroInTaskWithOutputOut"
            });
        }

        public Task OneInTaskWithNoOutputOut(Model1 input)
        {
            return new Task(() => { LastNameEntered = input.Name; });
        }

        public Model1 ZeroInOneOut()
        {
            return new Model1
            {
                Name = "ZeroInOneOut"
            };
        }

        public Model2 OneInOneOut(Model1 input)
        {
            return new Model2
            {
                Name = input.Name
            };
        }

        public Model2 OneInOneOutPartial(Model1 input)
        {
            return new Model2
            {
                Name = input.Name
            };
        }

        [FubuPartial]
        public Model2 OneInOneOutWithPartialAttribute(Model1 input)
        {
            return new Model2
            {
                Name = input.Name
            };
        }

        public void OneInZeroOut(Model1 input)
        {
            LastNameEntered = input.Name;
        }

        public bool BogusReturn()
        {
            return false;
        }

        public void BogusOneInput(int bogus)
        {
        }

        public void BogusMultiInput(Model1 input1, Model2 input2)
        {
        }

        public void GenericMethod<T>(List<T> list)
        {
        }

        [Wonky]
        public string get_wonky()
        {
            return "I'm wonky";
        }
    }

    public class WonkyAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCallBase call)
        {
            call.ParentChain().InsertFirst(Wrapper.For<WonkyWrapper>());
        }
    }

    public class WonkyWrapper : WrappingBehavior
    {
    }
}