using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Handlers;
using FubuMVC.StructureMap;
using FubuMVC.Tests.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuBootstrapperIntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            registry = new FubuRegistry(x => {
                x.Actions.IncludeType<TestController>();
            });

            

            container = new Container(x =>
            {
                x.For<ICurrentChain>().Use(new CurrentChain(null, null));
                x.For<IHttpRequest>().Use(OwinHttpRequest.ForTesting());
            });

            FubuMvcPackageFacility.PhysicalRootPath = AppDomain.CurrentDomain.BaseDirectory;

            routes = FubuApplication.For(registry)
                .StructureMap(container)
                .Bootstrap()
                .Routes
                .Where(r => !r.As<Route>().Url.StartsWith("_content"))
                .ToList();

            container.Configure(x => x.For<IOutputWriter>().Use(new InMemoryOutputWriter()));
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            FubuMvcPackageFacility.PhysicalRootPath = null;
        }

        private FubuRegistry registry;
        private Container container;
        private IList<RouteBase> routes;

        [Test]
        public void should_have_a_route_in_the_RouteCollection_with_a_Fubu_RouteHandler_for_each_route_in_the_registry()
        {
            routes.Each(x => x.ShouldBeOfType<Route>().RouteHandler.ShouldBeOfType<FubuRouteHandler>());
        }

        [Test]
        public void should_have_registered_behaviors_in_the_container()
        {
            (container.GetAllInstances<IActionBehavior>().Count() >= 6).ShouldBeTrue();
        }

        [Test]
        public void should_register_routes_in_order_of_the_number_of_their_inputs()
        {
            var urls = routes.OfType<Route>().Select(r => r.Url).Where(x => !x.Contains("hello"));

            // We're getting others in here because of the integration tests writing actionless views
            var expected = new[]
            {
                "area/sub2/prop",
                "area/sub4/some_pattern",
                "area/sub2/{Name}",
                "area/sub/{Name}/{Age}",
                "area/sub2/{Name}/{Age}",
                "area/sub3/{Name}/{Age}"
            };

            urls.Where(x => expected.Contains(x)).ShouldHaveTheSameElementsAs(
                expected);
        }

        public class TestController
        {
            [UrlPattern("area/sub2/prop")]
            public TestOutputModel Index()
            {
                return new TestOutputModel();
            }

            [UrlPattern("area/sub/{Name}/{Age}")]
            public TestOutputModel SomeAction(TestInputModel value)
            {
                return new TestOutputModel
                {
                    Prop1 = value.Prop1
                };
            }

            [UrlPattern("area/sub4/some_pattern")]
            public TestOutputModel SomeOtherAction(NotUsedModel not_used)
            {
                return new TestOutputModel();
            }

            [UrlPattern("area/sub2/{Name}/{Age}")]
            public TestOutputModel2 AnotherAction(TestInputModel value)
            {
                return new TestOutputModel2
                {
                    Prop1 = value.Prop1,
                    Name = value.Name,
                    Age = value.Age
                };
            }

            [UrlPattern("area/sub3/{Name}/{Age}")]
            public TestOutputModel3 ThirdAction(TestInputModel value)
            {
                return new TestOutputModel3
                {
                    Prop1 = value.Prop1
                };
            }

            [UrlPattern("area/sub2/{Name}")]
            public void RedirectAction(TestInputModel value)
            {
            }
        }
    }
}