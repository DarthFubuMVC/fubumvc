using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Tests.Registration.Conventions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ActionCallTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            action = ActionCall.For<ControllerTarget>(x => x.OneInZeroOut(null));
        }

        #endregion

        private ActionCall action;

        [Test]
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

        [Test]
        public void append_when_next_is_null()
        {
            action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            var next = new OutputNode(typeof (Model2));

            action.AddToEnd(next);

            action.Next.ShouldBeTheSameAs(next);
        }

        [Test]
        public void can_get_the_behavior_type()
        {
            ActionCall.For<ControllerTarget>(c => c.OneInOneOut(null))
                .BehaviorType.ShouldEqual(typeof (OneInOneOutActionInvoker<ControllerTarget, Model1, Model2>));
        }

        [Test]
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

        [Test]
        public void fail_to_build_an_action_by_type_for_a_type_with_more_than_one_method()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                () => { ActionCall.For(typeof (InvalidActionWithMultipleMethods)); });
        }

        [Test]
        public void next_is_a_null_by_default()
        {
            action.Next.ShouldBeNull();

            var objectDef = action.As<IContainerModel>().ToObjectDef();
            objectDef.Dependencies.Select(x => x as ConfiguredDependency).Count().ShouldEqual(1);
        }

        [Test]
        public void returns_T_should_tell_if_action_has_output_of_type()
        {
            action = ActionCall.For<ControllerTarget>(c => c.OneInOneOut(null));
            action.Returns<Model2>().ShouldBeTrue();
            action.Returns<object>().ShouldBeTrue();
            action.Returns<Model1>().ShouldBeFalse();
        }

        [Test]
        public void should_return_if_equal()
        {
            action.Equals(action).ShouldBeTrue();
            action.Equals(null).ShouldBeFalse();
            action.Equals((object) null).ShouldBeFalse();
            action.Equals("").ShouldBeFalse();
        }

        [Test]
        public void should_return_is_internal_fubu_action()
        {
            action.IsInternalFubuAction().ShouldBeFalse();
        }

        [Test]
        public void successfully_build_an_action_from_a_handler_type()
        {
            var action = ActionCall.For(typeof (ValidActionWithOneMethod));
            action.HandlerType.ShouldEqual(typeof (ValidActionWithOneMethod));
            action.Method.ShouldEqual(typeof (ValidActionWithOneMethod).GetMethod("Go"));
        }

        [Test]
        public void to_definition_with_an_input_type()
        {
            ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null))
                .ToRouteDefinition()
                .Input.ShouldBeOfType<RouteInput<Model1>>();
        }

        [Test]
        public void to_definition_with_no_input_type()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut());
            action.ToRouteDefinition().ShouldBeOfType<RouteDefinition>().Input.ShouldBeNull();
        }

        [Test]
        public void to_object_def_throws_when_has_no_return_and_no_input()
        {
            action = ActionCall.For<ControllerTarget>(x => x.ZeroInZeroOut());
            Exception<FubuException>.ShouldBeThrownBy(
                () => action.As<IContainerModel>().ToObjectDef())
                .ErrorCode.ShouldEqual(1005);
        }

        [Test]
        public void to_object_def_throws_when_has_task_with_no_result_and_no_input()
        {
            action = ActionCall.For<ControllerTarget>(x => x.ZeroInTaskNoResultOut());
            Exception<FubuException>.ShouldBeThrownBy(
                () => action.As<IContainerModel>().ToObjectDef())
                .ErrorCode.ShouldEqual(1005);
        }
    }

    [TestFixture]
    public class ActionCallValidationTester
    {
        [Test]
        public void add_before_must_be_idempotent()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));
            var newNode = new InputNode(typeof (Model1));

            action.AddBefore(newNode);

            action.PreviousNodes.Count().ShouldEqual(1);

            action.AddBefore(newNode);

            action.PreviousNodes.Count().ShouldEqual(1);
        }

        [Test]
        public void do_not_append_a_duplicate_node()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));

            // first one is ok
            var newNode = new InputNode(typeof (InputModel));
            action.AddToEnd(newNode);
            action.Count().ShouldEqual(1);

            // try it again, the second should be ignored
            action.AddToEnd(newNode);
            action.Count().ShouldEqual(1);
        }


        [Test]
        public void do_not_append_a_duplicate_node_2()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));

            // first one is ok
            var newNode = new InputNode(typeof (InputModel));
            action.AddToEnd(newNode);
            action.Count().ShouldEqual(1);

            action.AddToEnd(new Wrapper(typeof (Wrapper1)));

            // try it again, the second should be ignored
            action.AddToEnd(newNode);
            action.Count().ShouldEqual(2);
            action.Count(x => x is InputNode).ShouldEqual(1);
        }

        [Test]
        public void should_not_throw_if_call_is_OMIOMO()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            action.Validate();
        }

        [Test]
        public void should_not_throw_if_call_is_OMIZMO()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.OneInZeroOut(null));
            action.Validate();
        }

        [Test]
        public void should_not_throw_if_call_is_ZMIOMO()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut());
            action.Validate();
        }

        [Test]
        public void should_throw_if_input_type_is_value_type()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusOneInput(9));
            var ex = typeof (FubuException).ShouldBeThrownBy(action.Validate).ShouldBeOfType<FubuException>();
            ex.ErrorCode.ShouldEqual(1006);
        }

        [Test]
        public void should_throw_if_more_than_one_input_parameter()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));
            var ex = typeof (FubuException).ShouldBeThrownBy(action.Validate).ShouldBeOfType<FubuException>();
            ex.ErrorCode.ShouldEqual(1005);
        }

        [Test]
        public void should_throw_if_return_type_is_value_type()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusReturn());
            var ex = typeof (FubuException).ShouldBeThrownBy(action.Validate).ShouldBeOfType<FubuException>();
            ex.ErrorCode.ShouldEqual(1004);
        }
    }

    public class FakeNode : BehaviorNode
    {
        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }

        protected override ObjectDef buildObjectDef()
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


    public abstract class InvocationBuildingContext
    {
        private readonly MethodInfo method;
        private ActionCall definition;
        protected ObjectDef theObjectDef;

        protected InvocationBuildingContext(Expression<Func<ControllerTarget, object>> expression)
        {
            method = ReflectionHelper.GetMethod(expression);
        }

        protected InvocationBuildingContext(Expression<Action<ControllerTarget>> expression)
        {
            method = ReflectionHelper.GetMethod(expression);
        }

        [SetUp]
        public void SetUp()
        {
            definition = new ActionCall(typeof (ControllerTarget), method);

            theObjectDef = definition.As<IContainerModel>().ToObjectDef();
        }
    }

    [TestFixture]
    public class when_building_the_invocation_for_one_model_in_one_model_out : InvocationBuildingContext
    {
        public when_building_the_invocation_for_one_model_in_one_model_out()
            : base(x => x.OneInOneOut(null))
        {
        }

        [Test]
        public void should_have_a_dependency_for_the_function()
        {
            theObjectDef.Dependencies.Count().ShouldEqual(1);
            var dependency = theObjectDef.Dependencies.First();

            dependency.DependencyType.ShouldEqual(typeof (Func<ControllerTarget, Model1, Model2>));
        }

        [Test]
        public void the_dependency_function_invokes_the_correct_function()
        {
            var func = theObjectDef.Dependencies.First().ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeOfType<Func<ControllerTarget, Model1, Model2>>();

            var target = new ControllerTarget();
            func(target, new Model1{
                Name = "Jeremy"
            }).Name.ShouldEqual("Jeremy");
        }

        [Test]
        public void the_type_should_be_OMIOMO()
        {
            theObjectDef.Type.ShouldEqual(typeof (OneInOneOutActionInvoker<ControllerTarget, Model1, Model2>));
        }
    }

    [TestFixture]
    public class when_building_the_invocation_for_one_model_in_zero_model_out : InvocationBuildingContext
    {
        public when_building_the_invocation_for_one_model_in_zero_model_out()
            : base(x => x.OneInZeroOut(null))
        {
        }

        [Test]
        public void should_have_a_dependency_for_the_function()
        {
            theObjectDef.Dependencies.Count().ShouldEqual(1);
            var dependency = theObjectDef.Dependencies.First();

            dependency.DependencyType.ShouldEqual(typeof (Action<ControllerTarget, Model1>));
        }

        [Test]
        public void the_dependency_function_invokes_the_correct_function()
        {
            var func = theObjectDef.Dependencies.First().ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeOfType<Action<ControllerTarget, Model1>>();

            var target = new ControllerTarget();
            func(target, new Model1{
                Name = "Jeremy"
            });

            target.LastNameEntered.ShouldEqual("Jeremy");
        }

        [Test]
        public void the_type_should_be_OMIOMO()
        {
            theObjectDef.Type.ShouldEqual(typeof (OneInZeroOutActionInvoker<ControllerTarget, Model1>));
        }
    }

    [TestFixture]
    public class when_building_the_invocation_for_zero_model_in_one_model_out : InvocationBuildingContext
    {
        public when_building_the_invocation_for_zero_model_in_one_model_out()
            : base(x => x.ZeroInOneOut())
        {
        }

        [Test]
        public void should_have_a_dependency_for_the_function()
        {
            theObjectDef.Dependencies.Count().ShouldEqual(1);
            var dependency = theObjectDef.Dependencies.First();

            dependency.DependencyType.ShouldEqual(typeof (Func<ControllerTarget, Model1>));
        }

        [Test]
        public void the_dependency_function_invokes_the_correct_function()
        {
            var func = theObjectDef.Dependencies.First().ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeOfType<Func<ControllerTarget, Model1>>();

            var target = new ControllerTarget();
            func(target).Name.ShouldEqual("ZeroInOneOut");
        }

        [Test]
        public void the_type_should_be_OMIOMO()
        {
            theObjectDef.Type.ShouldEqual(typeof (ZeroInOneOutActionInvoker<ControllerTarget, Model1>));
        }
    }

    [TestFixture]
    public class when_building_the_invocation_for_one_model_in_task_with_no_output_out : InvocationBuildingContext
    {
        public when_building_the_invocation_for_one_model_in_task_with_no_output_out()
            : base(x => x.OneInTaskWithNoOutputOut(null))
        {
        }

        [Test]
        public void should_have_a_dependency_for_the_function()
        {
            theObjectDef.Dependencies.Count().ShouldEqual(1);
            var dependency = theObjectDef.Dependencies.First();

            dependency.DependencyType.ShouldEqual(typeof (Func<ControllerTarget, Model1, Task>));
        }

        [Test]
        public void the_dependency_function_invokes_the_correct_function()
        {
            var func = theObjectDef.Dependencies.First().ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeOfType<Func<ControllerTarget, Model1, Task>>();

            var target = new ControllerTarget();
            var task = func(target, new Model1{
                Name = "Corey"
            });
            task.RunSynchronously();

            target.LastNameEntered.ShouldEqual("Corey");
        }

        [Test]
        public void the_type_should_be_OMIOMO()
        {
            theObjectDef.Type.ShouldEqual(typeof (OneInOneOutActionInvoker<ControllerTarget, Model1, Task>));
        }
    }

    [TestFixture]
    public class when_building_the_invocation_for_zero_model_in_task_with_output_out : InvocationBuildingContext
    {
        public when_building_the_invocation_for_zero_model_in_task_with_output_out()
            : base(x => x.ZeroInTaskWithOutputOut())
        {
        }

        [Test]
        public void should_have_a_dependency_for_the_function()
        {
            theObjectDef.Dependencies.Count().ShouldEqual(1);
            var dependency = theObjectDef.Dependencies.First();

            dependency.DependencyType.ShouldEqual(typeof (Func<ControllerTarget, Task<Model2>>));
        }

        [Test]
        public void the_dependency_function_invokes_the_correct_function()
        {
            var func = theObjectDef.Dependencies.First().ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeOfType<Func<ControllerTarget, Task<Model2>>>();

            var target = new ControllerTarget();
            var task = func(target);
            task.RunSynchronously();
            task.Result.Name.ShouldEqual("ZeroInTaskWithOutputOut");
        }

        [Test]
        public void the_type_should_be_OMIOMO()
        {
            theObjectDef.Type.ShouldEqual(typeof (ZeroInOneOutActionInvoker<ControllerTarget, Task<Model2>>));
        }
    }

    [TestFixture]
    public class when_building_the_invocation_for_one_model_in_one_task_with_model_out : InvocationBuildingContext
    {
        public when_building_the_invocation_for_one_model_in_one_task_with_model_out()
            : base(x => x.OneInTaskWithOutputOut(null))
        {
        }

        [Test]
        public void should_have_a_dependency_for_the_function()
        {
            theObjectDef.Dependencies.Count().ShouldEqual(1);
            var dependency = theObjectDef.Dependencies.First();

            dependency.DependencyType.ShouldEqual(typeof (Func<ControllerTarget, Model1, Task<Model2>>));
        }

        [Test]
        public void the_dependency_function_invokes_the_correct_function()
        {
            var func = theObjectDef.Dependencies.First().ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeOfType<Func<ControllerTarget, Model1, Task<Model2>>>();

            var target = new ControllerTarget();
            var task = func(target, new Model1{
                Name = "Corey"
            });
            task.RunSynchronously();
            task.Result.Name.ShouldEqual("Corey");
        }

        [Test]
        public void the_type_should_be_OMIOMO()
        {
            theObjectDef.Type.ShouldEqual(typeof (OneInOneOutActionInvoker<ControllerTarget, Model1, Task<Model2>>));
        }

        [Test]
        public void build_chain_that_should_have_a_route()
        {
            var actionCall = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            var chain = actionCall.BuildChain(new UrlPolicies());
            chain.IsPartialOnly.ShouldBeFalse();

            chain.Top.ShouldBeTheSameAs(actionCall);
        }

        [Test]
        public void build_chain_for_a_partial_suffix()
        {
            var actionCall = ActionCall.For<ControllerTarget>(x => x.OneInOneOutPartial(null));
            var chain = actionCall.BuildChain(new UrlPolicies());
            chain.IsPartialOnly.ShouldBeTrue();

            chain.Top.ShouldBeTheSameAs(actionCall);
        }

        [Test]
        public void build_chain_for_an_action_decorated_with_the_FubuPartial_attribute()
        {
            var actionCall = ActionCall.For<ControllerTarget>(x => x.OneInOneOutWithPartialAttribute(null));
            var chain = actionCall.BuildChain(new UrlPolicies());
            chain.IsPartialOnly.ShouldBeTrue();

            chain.Top.ShouldBeTheSameAs(actionCall);
        }

        [Test]
        public void applies_modify_chain_attributes_to_the_created_chain()
        {
            var actionCall = ActionCall.For<ControllerTarget>(x => x.get_wonky());
            var chain = actionCall.BuildChain(new UrlPolicies());

            chain.IsWrappedBy(typeof(WonkyWrapper))
                .ShouldBeTrue();
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
            return new Task<Model2>(() => new Model2{
                Name = input.Name
            });
        }

        public Task<Model2> ZeroInTaskWithOutputOut()
        {
            return new Task<Model2>(() => new Model2{
                Name = "ZeroInTaskWithOutputOut"
            });
        }

        public Task OneInTaskWithNoOutputOut(Model1 input)
        {
            return new Task(() => { LastNameEntered = input.Name; });
        }

        public Model1 ZeroInOneOut()
        {
            return new Model1{
                Name = "ZeroInOneOut"
            };
        }

        public Model2 OneInOneOut(Model1 input)
        {
            return new Model2{
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
        public override void Alter(ActionCall call)
        {
            call.ParentChain().InsertFirst(Wrapper.For<WonkyWrapper>());
        }
    }

    public class WonkyWrapper : WrappingBehavior
    {
        
    }
}