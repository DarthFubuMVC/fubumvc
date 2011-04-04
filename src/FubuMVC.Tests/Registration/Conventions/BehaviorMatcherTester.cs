using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class BehaviorMatcherTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new BehaviorGraph(null);
            matcher = new BehaviorMatcher((type, methodInfo) => actionCallProvider(type, methodInfo));

            pool = new TypePool(null);
            pool.IgnoreCallingAssembly();

            pool.AddType<DifferentPatternClass>();
            pool.AddType<OneController>();
            pool.AddType<TwoController>();
            pool.AddType<ThreeController>();

            calls = null;
        }

        #endregion

        private Func<Type, MethodInfo, ActionCall> actionCallProvider = (type, methodInfo) => new ActionCall(type, methodInfo);
        private BehaviorGraph graph;
        private BehaviorMatcher matcher;
        private TypePool pool;
        private IEnumerable<ActionCall> calls;

        private void setFilters(Action action)
        {
            action();
            matcher.BuildBehaviors(pool, graph);
            calls = graph.Behaviors.SelectMany(x => x.Calls);
        }

        [Test]
        public void scan_with_a_filter_on_controller_implements_interface()
        {
            setFilters(() => { matcher.TypeFilters.Includes += t => t.IsConcreteTypeOf<IPattern>(); });

            graph.BehaviorChainCount.ShouldEqual(6);

            calls.Count(x => x.HandlerType == typeof (DifferentPatternClass)).ShouldEqual(3);
            calls.Count(x => x.HandlerType == typeof (OneController)).ShouldEqual(3);
            calls.Count(x => x.HandlerType == typeof (TwoController)).ShouldEqual(0);
            calls.Count(x => x.HandlerType == typeof (ThreeController)).ShouldEqual(0);
        }

        [Test]
        public void scan_with_a_filter_on_controller_name()
        {
            setFilters(() => { matcher.TypeFilters.Includes += t => t.Name.EndsWith("Controller"); });

            graph.Behaviors.Each(chain => Debug.WriteLine(chain.Calls.First()));

            graph.BehaviorChainCount.ShouldEqual(9);

            calls.Count(x => x.HandlerType == typeof (DifferentPatternClass)).ShouldEqual(0);
            calls.Count(x => x.HandlerType == typeof (OneController)).ShouldEqual(3);
            calls.Count(x => x.HandlerType == typeof (TwoController)).ShouldEqual(3);
            calls.Count(x => x.HandlerType == typeof (ThreeController)).ShouldEqual(3);
        }

        [Test]
        public void scan_with_a_filter_to_exclude_method()
        {
            setFilters(() =>
            {
                matcher.TypeFilters.Includes += type => type.IsConcrete();
                matcher.MethodFilters.Excludes += call => call.Method.Name.Contains("Go");
            });

            graph.BehaviorChainCount.ShouldEqual(8);

            calls.Count(x => x.HandlerType == typeof (DifferentPatternClass)).ShouldEqual(2);
            calls.Count(x => x.HandlerType == typeof (OneController)).ShouldEqual(2);
            calls.Count(x => x.HandlerType == typeof (TwoController)).ShouldEqual(2);
            calls.Count(x => x.HandlerType == typeof (ThreeController)).ShouldEqual(2);
        }

        [Test]
        public void scan_with_no_filters_other_than_concrete_should_have_every_public_method_of_the_concrete_types()
        {
            setFilters(() => { matcher.TypeFilters.Includes += type => type.IsConcrete(); });


            graph.BehaviorChainCount.ShouldEqual(12);
            calls.Any(x => x.HandlerType == typeof (IPattern)).ShouldBeFalse();

            calls.Count(x => x.HandlerType == typeof (DifferentPatternClass)).ShouldEqual(3);
            calls.Count(x => x.HandlerType == typeof (OneController)).ShouldEqual(3);
            calls.Count(x => x.HandlerType == typeof (TwoController)).ShouldEqual(3);
            calls.Count(x => x.HandlerType == typeof (ThreeController)).ShouldEqual(3);
        }

        [Test]
        public void can_set_a_different_action_call_provider()
        {
            actionCallProvider = (type, methodInfo) => new TestActionCall(type, methodInfo);

            matcher.BuildBehaviors(pool, graph);

            graph.Behaviors
                .Select(x => x.FirstCall()).All(x => x.GetType() == typeof (TestActionCall))
                .ShouldBeTrue();
        }
    }

    public class SimpleInputModel
    {
    }

    public class SimpleOutputModel
    {
    }

    public class DifferentPatternClass : IPattern
    {
        public string Name { get; set; }

        public void Go()
        {
        }

        public SimpleOutputModel Report()
        {
            return new SimpleOutputModel();
        }

        public SimpleOutputModel Query(SimpleInputModel model)
        {
            return new SimpleOutputModel();
        }

        private void Go2()
        {
        }

        public void Go<T>()
        {
        }
    }

    public interface IPattern
    {
        void Go();
        SimpleOutputModel Report();
        SimpleOutputModel Query(SimpleInputModel model);
    }

    public class OneController : IPattern
    {
        public void Go()
        {
        }


        public SimpleOutputModel Report()
        {
            return new SimpleOutputModel();
        }

        public SimpleOutputModel Query(SimpleInputModel model)
        {
            return new SimpleOutputModel();
        }

        public static void MethodThatShouldNotBeHere()
        {
        }
    }

    public class TwoController
    {
        public void Go()
        {
        }

        public SimpleOutputModel Report()
        {
            return new SimpleOutputModel();
        }

        public SimpleOutputModel Query(SimpleInputModel model)
        {
            return new SimpleOutputModel();
        }
    }

    public class ThreeController
    {
        public void Go()
        {
        }

        public SimpleOutputModel Report()
        {
            return new SimpleOutputModel();
        }

        public SimpleOutputModel Query(SimpleInputModel model)
        {
            return new SimpleOutputModel();
        }
    }

    public class TestActionCall : ActionCall
    {
        public TestActionCall(Type handlerType, MethodInfo method) : base(handlerType, method)
        {
        }

        protected override Core.Registration.ObjectGraph.ObjectDef buildObjectDef()
        {
            //replace IActionBehavior with something other than one/zero in/out action invoker
            return new ObjectDef
            {
                Type = typeof(TestActionBehavior)
            };
        }
    }

    public class TestActionBehavior : BasicBehavior
    {
        public TestActionBehavior(PartialBehavior partialBehavior) : base(partialBehavior)
        {
        }
    }
}