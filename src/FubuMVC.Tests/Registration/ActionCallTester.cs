using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Util;
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
    }

    [TestFixture]
    public class ActionCallValidationTester
    {
        [Test]
        public void should_throw_if_return_type_is_value_type()
        {
            var action = ActionCall.For<ControllerTarget>(x => x.BogusReturn());
            
            action.Validate();

            Assert.Fail("Need to implement this test");
        }

        [Test]
        public void should_throw_if_input_type_is_value_type()
        {
            Assert.Fail("Need to implement this test");
        }

        [Test]
        public void should_throw_if_more_than_one_input_parameter()
        {
            Assert.Fail("Need to implement this test");
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
    }
}