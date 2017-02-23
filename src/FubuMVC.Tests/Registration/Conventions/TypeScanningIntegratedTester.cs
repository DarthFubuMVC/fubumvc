using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Tests.Behaviors;
using Shouldly;
using Xunit;
using TestPackage1.FakeControllers;

namespace FubuMVC.Tests.Registration.Conventions
{
    
    public class TypeScanningIntegratedTester
    {

        [Fact]
        public void pick_up_behaviors_from_another_assembly()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.DisableDefaultActionSource();
                x.Actions.FindBy(o =>
                {
                    o.Applies.ToAssemblyContainingType<ClassInAnotherAssembly>();
                    o.IncludeTypesNamed(name => name.EndsWith("Controller"));
                });
            });

            graph.Chains.Count().ShouldBeGreaterThan(0);

            graph.Chains
                .Where(x => x.Calls.Any())
                .Where(x => x.FirstCall().HandlerType.Assembly != typeof (BehaviorGraph).Assembly)
                .Each(x =>
                {
                    x.Calls.First().HandlerType.Name.EndsWith("Controller");
                    x.Calls.First().HandlerType.Assembly.ShouldBe(typeof (ClassInAnotherAssembly).Assembly);
                });
        }

        [Fact]
        public void pick_up_behaviors_from_this_assembly()
        {
            var graph = BehaviorGraph.BuildFrom(x => x.Actions.IncludeClassesSuffixedWithController());

            graph.Chains.Count().ShouldBeGreaterThan(0);
            graph.Chains.OfType<RoutedChain>().Where(x => x.Calls.Any()).Each(x => x.Calls.First().HandlerType.Name.EndsWith("Controller"));
        }

        [Fact]
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