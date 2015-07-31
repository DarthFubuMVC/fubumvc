using System;
using System.Reflection;
using AssemblyPackage;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class ActionSourceTester
    {
        private Lazy<BehaviorGraph> _graph;
        private ActionSource source;

        [SetUp]
        public void SetUp()
        {
            source = new ActionSource();
            _graph =
                new Lazy<BehaviorGraph>(() => BehaviorGraph.BuildFrom(r =>
                {
                    r.Actions.DisableDefaultActionSource();
                    r.Actions.FindWith(source);
                }));
        }

        private BehaviorGraph theResultingGraph
        {
            get { return _graph.Value; }
        }


        [Test]
        public void uses_the_application_assembly_if_none_is_specified()
        {
            source.IncludeClassesSuffixedWithController();

            theResultingGraph.ApplicationAssembly.GetName()
                .Name.ShouldBe(Assembly.GetExecutingAssembly().GetName().Name);

            theResultingGraph.ChainFor<OneController>(x => x.Query(null))
                .ShouldNotBeNull();
        }

        [Test]
        public void does_not_use_The_application_assembly_if_other_assemblies_are_specified()
        {
            source.Applies.ToAssemblyContainingType<AssemblyEndpoint>();

            theResultingGraph.ChainFor<OneController>(x => x.Query(null))
                .ShouldBeNull();
        }

        [Test]
        public void does_find_actions_from_other_assemblies()
        {
            source.Applies.ToAssemblyContainingType<AssemblyEndpoint>();
            source.IncludeClassesSuffixedWithEndpoint();

            theResultingGraph.ChainFor<AssemblyEndpoint>(x => x.get_hello())
                .ShouldNotBeNull();
        }

        [Test]
        public void match_by_endpoint()
        {
            source.IncludeClassesSuffixedWithEndpoint();

            theResultingGraph.ChainFor<OneEndpoint>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.ChainFor<OneController>(x => x.Report()).ShouldBeNull();
        }

        [Test]
        public void match_by_controller_suffix()
        {
            source.IncludeClassesSuffixedWithController();

            theResultingGraph.ChainFor<OneEndpoint>(x => x.Report()).ShouldBeNull();
            theResultingGraph.ChainFor<OneController>(x => x.Report()).ShouldNotBeNull();
        }

        [Test]
        public void match_by_name()
        {
            source.IncludeTypesNamed(x => x.StartsWith("One"));

            theResultingGraph.ChainFor<OneEndpoint>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.ChainFor<OneController>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.ChainFor<TwoEndpoint>(x => x.Report()).ShouldBeNull();
        }

        [Test]
        public void match_by_types()
        {
            source.IncludeTypes(x => x.Name.StartsWith("One"));

            theResultingGraph.ChainFor<OneEndpoint>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.ChainFor<OneController>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.ChainFor<TwoEndpoint>(x => x.Report()).ShouldBeNull();
        }

        [Test]
        public void match_by_types_implementing()
        {
            source.IncludeTypesImplementing<IPattern>();

            theResultingGraph.ChainFor<DifferentPatternClass>(x => x.Report())
                .ShouldNotBeNull();
        }

        [Test]
        public void include_methods()
        {
            source.IncludeClassesSuffixedWithController();
            source.IncludeMethods(x => x.Name.StartsWith("Q"));

            theResultingGraph.ChainFor<OneController>(x => x.Query(null)).ShouldNotBeNull();
            theResultingGraph.ChainFor<OneController>(x => x.Report()).ShouldBeNull();
        }

        [Test]
        public void exclude_types()
        {
            source.IncludeClassesSuffixedWithController();
            source.ExcludeTypes(x => x == typeof (TwoController));

            theResultingGraph.ChainFor<OneController>(x => x.Query(null)).ShouldNotBeNull();
            theResultingGraph.ChainFor<TwoController>(x => x.Query(null)).ShouldBeNull();
        }

        [Test]
        public void exclude_methods()
        {
            source.IncludeClassesSuffixedWithController();
            source.ExcludeMethods(x => x.Name.StartsWith("Q"));

            theResultingGraph.ChainFor<OneController>(x => x.Query(null)).ShouldBeNull();
            theResultingGraph.ChainFor<OneController>(x => x.Report()).ShouldNotBeNull();
        }

        [Test]
        public void ignore_methods_declared_by()
        {
            source.IncludeClassesSuffixedWithController();
            source.IgnoreMethodsDeclaredBy<OneController>();

            theResultingGraph.ChainFor<ChildController>(x => x.Report()).ShouldBeNull();
            theResultingGraph.ChainFor<ChildController>(x => x.ChildQuery(null)).ShouldNotBeNull();
        }

        [Test]
        public void exclude_non_concrete_types()
        {
            source.IncludeTypes(x => x.CanBeCastTo<IPattern>());
            source.ExcludeNonConcreteTypes();

            theResultingGraph.ChainFor<IPattern>(x => x.Query(null))
                .ShouldBeNull();
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

    public class OneEndpoint
    {
        public SimpleOutputModel Report()
        {
            return new SimpleOutputModel();
        }
    }

    public class TwoEndpoint
    {
        public SimpleOutputModel Report()
        {
            return new SimpleOutputModel();
        }
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

    public class ChildController : OneController
    {
        public SimpleOutputModel ChildQuery(SimpleInputModel model)
        {
            return new SimpleOutputModel();
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

        protected override IConfiguredInstance buildInstance()
        {
            return new SmartInstance<TestActionBehavior>();
        }

    }

    public class TestActionBehavior : BasicBehavior
    {
        public TestActionBehavior(PartialBehavior partialBehavior) : base(partialBehavior)
        {
        }
    }
}