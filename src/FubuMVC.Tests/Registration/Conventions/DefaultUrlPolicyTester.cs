using System;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.PathBased;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class DefaultUrlPolicyTester
    {

        private IRouteDefinition routeFor<T>(Expression<Action<T>> expression)
        {
            var call = ActionCall.For(expression);
            return new DefaultUrlPolicy().Build(call);
        }

        [Test]
        public void build_route_for_endpoint_method()
        {
            var route = routeFor<SomethingEndpoint>(x => x.Go(null));
            route.Pattern.ShouldEqual("something/go/{Name}/{Age}");

            route.Input.QueryParameters.Single().Name.ShouldEqual("Level");

            route.Input.RouteParameters.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Test]
        public void build_route_for_endpoints_method()
        {
            var route = routeFor<SomethingEndpoints>(x => x.Go(null));
            route.Pattern.ShouldEqual("something/go/{Name}/{Age}");

            route.Input.QueryParameters.Single().Name.ShouldEqual("Level");

            route.Input.RouteParameters.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Test]
        public void build_route_for_controller_method()
        {
            var route = routeFor<SomethingController>(x => x.Go(null));
            route.Pattern.ShouldEqual("something/go/{Name}/{Age}");

            route.Input.QueryParameters.Single().Name.ShouldEqual("Level");

            route.Input.RouteParameters.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Test]
        public void build_route_for_Get_method()
        {
            var route = routeFor<SomethingEndpoint>(x => x.Get());
            route.Pattern.ShouldEqual("something");
            route.AllowedHttpMethods.Single().ShouldEqual("GET");
        }

        [Test]
        public void build_route_for_get_method_with_input()
        {
            var route = routeFor<SomethingEndpoints>(x => x.Get(null));
            route.Pattern.ShouldEqual("something/{Name}/{Age}");
            route.AllowedHttpMethods.Single().ShouldEqual("GET");
        }

        [Test]
        public void resource_path()
        {
            var route = routeFor<SomethingEndpoint>(x => x.Write(null));
            route.Pattern.ShouldEqual("something/write/" + ResourcePath.UrlSuffix);
        }


        public class SomethingEndpoint
        {
            public void Go(SomeInput input)
            {
                
            }

            public string Write(ResourcePath path)
            {
                return "anything";
            }

            public string Get()
            {
                return "Foo";
            }
        }

        public class SomethingEndpoints
        {
            public void Go(SomeInput input)
            {

            }

            public string Get(SomeInput input)
            {
                return "Bar";
            }
        }

        public class SomethingController
        {
            public void Go(SomeInput input)
            {

            }
        }

        public class SomeInput
        {
            [RouteInput]
            public string Name { get; set; }
            public string Direction { get; set; }
            
            [QueryString]
            public int Level { get; set; }
            
            [RouteInput]
            public int Age { get; set; }
        }
    }


}