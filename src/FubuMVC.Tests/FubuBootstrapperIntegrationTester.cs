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
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Handlers;
using Shouldly;
using Xunit;
using StructureMap;

namespace FubuMVC.Tests
{
    
    public class FubuBootstrapperIntegrationTester : IDisposable
    {
        public FubuBootstrapperIntegrationTester()
        {
            registry = new FubuRegistry(x => { x.Actions.IncludeType<TestController>(); });


            container = new Container(x =>
            {
                x.For<ICurrentChain>().Use(new CurrentChain(null, null));
                x.For<IHttpRequest>().Use(OwinHttpRequest.ForTesting());
            });

            registry.StructureMap(container);
            registry.RootPath = AppDomain.CurrentDomain.BaseDirectory;

            fubuRuntime = registry.ToRuntime();
            routes = fubuRuntime
                .Routes
                .Where(r => !r.As<Route>().Url.StartsWith("_content"))
                .ToList();

            container.Configure(x => x.For<IOutputWriter>().Use(new InMemoryOutputWriter()));
        }

        public void Dispose()
        {
            fubuRuntime.Dispose();
        }



        private FubuRegistry registry;
        private Container container;
        private IList<RouteBase> routes;
        private FubuRuntime fubuRuntime;

        [Fact]
        public void should_have_a_route_in_the_RouteCollection_with_a_Fubu_RouteHandler_for_each_route_in_the_registry()
        {
            routes.Each(x => x.ShouldBeOfType<Route>().RouteHandler.ShouldBeOfType<FubuRouteHandler>());
        }

        [Fact]
        public void should_have_registered_behaviors_in_the_container()
        {
            Debug.WriteLine(container.WhatDoIHave());

            (container.Model.For<IActionBehavior>().Instances.Count() >= 6).ShouldBeTrue();
        }

        [Fact]
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