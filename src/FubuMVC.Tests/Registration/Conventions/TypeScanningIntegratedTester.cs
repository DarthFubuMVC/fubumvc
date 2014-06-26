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
using FubuMVC.Tests.Behaviors;
using FubuTestingSupport;
using NUnit.Framework;
using TestPackage1.FakeControllers;

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
        public void pick_up_behaviors_from_another_assembly()
        {
            var graph = BehaviorGraph.BuildFrom(x => {
                x.Actions.FindBy(o => {
                    o.Applies.ToAssemblyContainingType<ClassInAnotherAssembly>();
                    o.IncludeTypesNamed(name => name.EndsWith("Controller"));
                });


            });

            graph.Behaviors.Count().ShouldBeGreaterThan(0);

            graph.Behaviors
                .Where(x => x.FirstCall().HandlerType.Assembly != typeof(BehaviorGraph).Assembly)
                .Each(x =>
            {
                x.Calls.First().HandlerType.Name.EndsWith("Controller");
                x.Calls.First().HandlerType.Assembly.ShouldEqual(typeof (ClassInAnotherAssembly).Assembly);
            });


        }

        [Test]
        public void pick_up_behaviors_from_this_assembly()
        {
            var graph = BehaviorGraph.BuildFrom(x => {
                x.Actions.IncludeClassesSuffixedWithController();
            });

            graph.Behaviors.Count().ShouldBeGreaterThan(0);
            graph.Behaviors.Each(x => x.Calls.First().HandlerType.Name.EndsWith("Controller"));

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