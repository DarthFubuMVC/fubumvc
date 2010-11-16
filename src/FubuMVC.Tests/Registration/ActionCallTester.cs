using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Tests.Registration.Conventions;
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
        public void append_json()
        {
            action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            action.AddToEnd(new RenderJsonNode(action.OutputType()));

            action.Next.ShouldBeOfType<RenderJsonNode>().ModelType.ShouldEqual(action.OutputType());
        }

        [Test]
        public void append_to_descendent_when_next_is_not_null()
        {
            action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            var wrapper = new Wrapper(typeof (FakeBehavior));
            action.AddAfter(wrapper);

            var next = new RenderJsonNode(typeof (Model2));

            action.AddToEnd(next);

            action.Next.ShouldBeTheSameAs(wrapper);
            wrapper.Next.ShouldBeTheSameAs(next);

            action.ShouldHaveTheSameElementsAs(wrapper, next);
        }

        [Test]
        public void append_when_next_is_null()
        {
            action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            var next = new RenderJsonNode(typeof (Model2));

            action.AddToEnd(next);

            action.Next.ShouldBeTheSameAs(next);
        }

        [Test]
        public void enrich_puts_the_new_chain_node_directly_behind_the_call()
        {
            action = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));
            var next = new RenderJsonNode(typeof (Model2));


            action.AddToEnd(next);


            var enricher = new Wrapper(typeof (string));
            action.AddAfter(enricher);

            action.Next.ShouldBeTheSameAs(enricher);
            enricher.Next.ShouldBeTheSameAs(next);
        }

        [Test]
        public void next_is_a_null_by_default()
        {
            action.Next.ShouldBeNull();

            ObjectDef objectDef = action.ToObjectDef();
            objectDef.Dependencies.Select(x => x as ConfiguredDependency).Count().ShouldEqual(1);
        }

        [Test]
        public void to_object_def_throws_when_has_no_return_and_no_input()
        {
            action = ActionCall.For<ControllerTarget>(x => x.ZeroInZeroOut());
            Exception<FubuException>.ShouldBeThrownBy(() => action.ToObjectDef())
                .ErrorCode.ShouldEqual(1005);
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
            action.Equals((object)null).ShouldBeFalse();
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
        public void fail_to_build_an_action_by_type_for_a_type_with_more_than_one_method()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                ActionCall.For(typeof (InvalidActionWithMultipleMethods));
            });
        }

    }

    [TestFixture]
    public class ActionCallValidationTester
    {
        [Test]
        public void should_not_throw_if_call_is_ZMIOMO()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut());
            action.Validate();
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
        public void should_throw_if_return_type_is_value_type()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusReturn());
            var ex = typeof (FubuException).ShouldBeThrownBy(action.Validate).ShouldBeOfType<FubuException>();
            ex.ErrorCode.ShouldEqual(1004);
        }

        [Test]
        public void should_throw_if_more_than_one_input_parameter()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));
            var ex = typeof(FubuException).ShouldBeThrownBy(action.Validate).ShouldBeOfType<FubuException>();
            ex.ErrorCode.ShouldEqual(1005);
        }

        [Test]
        public void should_throw_if_input_type_is_value_type()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusOneInput(9));
            var ex = typeof(FubuException).ShouldBeThrownBy(action.Validate).ShouldBeOfType<FubuException>();
            ex.ErrorCode.ShouldEqual(1006);
        }

        [Test]
        public void do_not_append_a_duplicate_node()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));

            // first one is ok
            action.AddToEnd(new DeserializeJsonNode(typeof(InputModel)));
            action.Count().ShouldEqual(1);

            // try it again, the second should be ignored
            action.AddToEnd(new DeserializeJsonNode(typeof(InputModel)));
            action.Count().ShouldEqual(1);
        }


        [Test]
        public void do_not_append_a_duplicate_node_2()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));

            // first one is ok
            action.AddToEnd(new DeserializeJsonNode(typeof(InputModel)));
            action.Count().ShouldEqual(1);

            action.AddToEnd(new Wrapper(typeof(Wrapper1)));

            // try it again, the second should be ignored
            action.AddToEnd(new DeserializeJsonNode(typeof(InputModel)));
            action.Count().ShouldEqual(2);
            action.Count(x => x is DeserializeJsonNode).ShouldEqual(1);
        }

        [Test]
        public void add_before_must_be_idempotent()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusMultiInput(null, null));
            action.AddBefore(new DeserializeJsonNode(typeof(Model1)));

            action.PreviousNodes.Count().ShouldEqual(1);

            action.AddBefore(new DeserializeJsonNode(typeof(Model1)));
            action.PreviousNodes.Count().ShouldEqual(1);
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

            theObjectDef = definition.ToObjectDef();
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
            theObjectDef.Dependencies.Count.ShouldEqual(1);
            IDependency dependency = theObjectDef.Dependencies.First();

            dependency.DependencyType.ShouldEqual(typeof (Func<ControllerTarget, Model1, Model2>));
        }

        [Test]
        public void the_dependency_function_invokes_the_correct_function()
        {
            var func = theObjectDef.Dependencies.First().ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeOfType<Func<ControllerTarget, Model1, Model2>>();

            var target = new ControllerTarget();
            func(target, new Model1
            {
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
            theObjectDef.Dependencies.Count.ShouldEqual(1);
            IDependency dependency = theObjectDef.Dependencies.First();

            dependency.DependencyType.ShouldEqual(typeof (Action<ControllerTarget, Model1>));
        }

        [Test]
        public void the_dependency_function_invokes_the_correct_function()
        {
            var func = theObjectDef.Dependencies.First().ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeOfType<Action<ControllerTarget, Model1>>();

            var target = new ControllerTarget();
            func(target, new Model1
            {
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
            theObjectDef.Dependencies.Count.ShouldEqual(1);
            IDependency dependency = theObjectDef.Dependencies.First();

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

  

    public class ValidActionWithOneMethod
    {
        public void Go(){}
    }

    public class InvalidActionWithMultipleMethods
    {
        public void Go() { }
        public void Go2() { }
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

        public void ZeroInZeroOut(){}

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

        public void OneInZeroOut(Model1 input)
        {
            LastNameEntered = input.Name;
        }

        public bool BogusReturn(){ return false; }

        public void BogusOneInput(int bogus){}

        public void BogusMultiInput(Model1 input1, Model2 input2){}

        public void GenericMethod<T>(List<T> list)
        {
        }
    }
}