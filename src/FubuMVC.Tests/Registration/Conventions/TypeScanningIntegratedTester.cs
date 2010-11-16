using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.FakeControllers;
using FubuMVC.Tests.Behaviors;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class TypeScanningIntegratedTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void limit_behaviors_by_policy_on_interface_methods()
        {
            BehaviorGraph graph = new FubuRegistry(x =>
            {
                x.Actions.ExcludeNonConcreteTypes().ForTypesOf<IRouter>(o => { o.Include(c => c.Go()); });
            }).BuildGraph();

            IEnumerable<string> calls = graph.Behaviors.Select(x => x.Calls.First().Method).Select(m =>
                "{0} - {1}".ToFormat(m.DeclaringType.Name, m.Name));

            calls.ShouldHaveTheSameElementsAs("Router1 - Go", "Router2 - Go", "Router3 - Go");
        }

        [Test]
        public void pick_up_behaviors_from_another_assembly()
        {
            BehaviorGraph graph = new FubuRegistry(x =>
            {
                
                x.Applies.ToAssemblyContainingType<ClassInAnotherAssembly>();
                x.Actions.IncludeTypesNamed(name => name.EndsWith("Controller"));
            }).BuildGraph();

            graph.BehaviorChainCount.ShouldBeGreaterThan(0);

            graph.Behaviors.Each(x =>
            {
                x.Calls.First().HandlerType.Name.EndsWith("Controller");
                x.Calls.First().HandlerType.Assembly.ShouldEqual(typeof (ClassInAnotherAssembly).Assembly);
            });


            graph.Routes.Each(x => Debug.WriteLine(x.Pattern));
        }

        [Test]
        public void pick_up_behaviors_from_this_assembly()
        {
            BehaviorGraph graph = new FubuRegistry(x =>
            {
                x.Actions.IncludeTypesNamed(name => name.EndsWith("Controller"));
            }).BuildGraph();

            graph.BehaviorChainCount.ShouldBeGreaterThan(0);
            graph.Behaviors.Each(x => x.Calls.First().HandlerType.Name.EndsWith("Controller"));

            graph.Routes.Each(x => Debug.WriteLine(x.Pattern));
        }

        [Test]
        public void TypeMethodPolicyTester()
        {
            var filter = new CompositeFilter<MethodInfo>();
            var policy = new TypeMethodPolicy<IRouter>(filter);
            policy.Include(x => x.Go());

            filter.Matches(ReflectionHelper.GetMethod<Router1>(x => x.Go())).ShouldBeTrue();
            filter.Matches(ReflectionHelper.GetMethod<Router1>(x => x.CanGo())).ShouldBeFalse();
        }
    }


    public interface IMarker
    {
    }

    public class ScannedMarkedClass : IMarker
    {
        public Output Go()
        {
            return null;
        }
    }

    public class SpecialScannedClass
    {
        public Output Go()
        {
            return null;
        }
    }

    public interface IRouter
    {
        void Go();
        bool CanGo();
    }

    public class Router1 : IRouter
    {
        public void Go()
        {
            throw new NotImplementedException();
        }

        public bool CanGo()
        {
            throw new NotImplementedException();
        }
    }

    public class Router2 : IRouter
    {
        public void Go()
        {
            throw new NotImplementedException();
        }

        public bool CanGo()
        {
            throw new NotImplementedException();
        }
    }

    public class Router3 : IRouter
    {
        public void Go()
        {
            throw new NotImplementedException();
        }

        public bool CanGo()
        {
            throw new NotImplementedException();
        }
    }
}