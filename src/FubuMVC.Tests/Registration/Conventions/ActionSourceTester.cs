using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using AssemblyPackage;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTestingSupport;
using NUnit.Framework;

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
            _graph = new Lazy<BehaviorGraph>(() => {
                return BehaviorGraph.BuildFrom(r => {
                    r.Actions.FindWith(source);
                });
            });
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
                .Name.ShouldEqual(Assembly.GetExecutingAssembly().GetName().Name);

            theResultingGraph.BehaviorFor<OneController>(x => x.Query(null))
                .ShouldNotBeNull();
        }

        [Test]
        public void does_not_use_The_application_assembly_if_other_assemblies_are_specified()
        {
            source.Applies.ToAssemblyContainingType<AssemblyPackage.AssemblyEndpoint>();

            theResultingGraph.BehaviorFor<OneController>(x => x.Query(null))
                .ShouldBeNull();
        }

        [Test]
        public void does_find_actions_from_other_assemblies()
        {
            source.Applies.ToAssemblyContainingType<AssemblyPackage.AssemblyEndpoint>();
            source.IncludeClassesSuffixedWithEndpoint();

            theResultingGraph.BehaviorFor<AssemblyEndpoint>(x => x.get_hello())
                .ShouldNotBeNull();
        }

        [Test]
        public void match_by_endpoint()
        {
            source.IncludeClassesSuffixedWithEndpoint();

            theResultingGraph.BehaviorFor<OneEndpoint>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.BehaviorFor<OneController>(x => x.Report()).ShouldBeNull();
        }

        [Test]
        public void match_by_controller_suffix()
        {
            source.IncludeClassesSuffixedWithController();

            theResultingGraph.BehaviorFor<OneEndpoint>(x => x.Report()).ShouldBeNull();
            theResultingGraph.BehaviorFor<OneController>(x => x.Report()).ShouldNotBeNull();
        }

        [Test]
        public void match_by_name()
        {
            source.IncludeTypesNamed(x => x.StartsWith("One"));

            theResultingGraph.BehaviorFor<OneEndpoint>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.BehaviorFor<OneController>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.BehaviorFor<TwoEndpoint>(x => x.Report()).ShouldBeNull();
        }

        [Test]
        public void match_by_types()
        {
            source.IncludeTypes(x => x.Name.StartsWith("One"));

            theResultingGraph.BehaviorFor<OneEndpoint>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.BehaviorFor<OneController>(x => x.Report()).ShouldNotBeNull();
            theResultingGraph.BehaviorFor<TwoEndpoint>(x => x.Report()).ShouldBeNull();
        }

        [Test]
        public void match_by_types_implementing()
        {
            source.IncludeTypesImplementing<IPattern>();

            theResultingGraph.BehaviorFor<DifferentPatternClass>(x => x.Report())
                .ShouldNotBeNull();
        }

        [Test]
        public void include_methods()
        {
            source.IncludeClassesSuffixedWithController();
            source.IncludeMethods(x => x.Name.StartsWith("Q"));

            theResultingGraph.BehaviorFor<OneController>(x => x.Query(null)).ShouldNotBeNull();
            theResultingGraph.BehaviorFor<OneController>(x => x.Report()).ShouldBeNull();
        }

        [Test]
        public void exclude_types()
        {
            source.IncludeClassesSuffixedWithController();
            source.ExcludeTypes(x => x == typeof(TwoController));

            theResultingGraph.BehaviorFor<OneController>(x => x.Query(null)).ShouldNotBeNull();
            theResultingGraph.BehaviorFor<TwoController>(x => x.Query(null)).ShouldBeNull();

        }

        [Test]
        public void exclude_methods()
        {
            source.IncludeClassesSuffixedWithController();
            source.ExcludeMethods(x => x.Name.StartsWith("Q"));

            theResultingGraph.BehaviorFor<OneController>(x => x.Query(null)).ShouldBeNull();
            theResultingGraph.BehaviorFor<OneController>(x => x.Report()).ShouldNotBeNull();
        }

        [Test]
        public void ignore_methods_declared_by()
        {
            source.IncludeClassesSuffixedWithController();
            source.IgnoreMethodsDeclaredBy<OneController>();

            theResultingGraph.BehaviorFor<ChildController>(x => x.Report()).ShouldBeNull();
            theResultingGraph.BehaviorFor<ChildController>(x => x.ChildQuery(null)).ShouldNotBeNull();
        }

        [Test]
        public void exclude_non_concrete_types()
        {
            source.IncludeTypes(x => x.CanBeCastTo<IPattern>());
            source.ExcludeNonConcreteTypes();

            theResultingGraph.BehaviorFor<IPattern>(x => x.Query(null))
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