using System;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class when_adding_routes_with_an_input_type_route_input_policy
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x =>
            {
                x.Actions.IncludeTypesNamed(o => o.StartsWith("Special"));

                x.Routes
                    .IgnoreNamespaceForUrlFrom<SpecialMessage>()
                    .ForInputTypesOf<SpecialMessage>(o => o.RouteInputFor(y => y.Id))
                    .IgnoreSpecificInputForInputTypeAndMethod<SpecialRouteInputTestingMessage>(
                    c => c.Method.Name == "NoId", o => o.Id);
            }).BuildGraph();
        }

        #endregion

        private BehaviorGraph graph;

        public class SpecialMessage
        {
            public Guid Id { get; set; }
        }

        public class SpecialMessage1 : SpecialMessage
        {
        }

        public class SpecialMessage2 : SpecialMessage
        {
        }

        public class SpecialMessage3 : SpecialMessage
        {
        }

        public class SpecialController
        {
            public void Go1(InputModel model)
            {
            }

            public void Go2(SpecialMessage1 model)
            {
            }

            public void Go3(SpecialMessage2 model)
            {
            }

            public void Go4(SpecialMessage3 model)
            {
            }

            public void NeedsId(SpecialRouteInputTestingMessage model)
            {
            }

            public void NoId(SpecialRouteInputTestingMessage model)
            {
            }
        }

        public class SpecialRouteInputTestingMessage
        {
            [RouteInput("")]
            public Guid Id { get; set; }

            [RouteInput("")]
            public string ExtraParam { get; set; }
        }

        [Test]
        public void
            each_route_with_the_input_type_matching_the_policy_should_have_the_route_inputs_specified_in_the_convention()
        {
            graph.RouteFor<SpecialController>(x => x.Go1(null)).As<RouteDefinition<InputModel>>().RouteInputs.Count.
                ShouldEqual(0);
            graph.RouteFor<SpecialController>(x => x.Go2(null)).As<RouteDefinition<SpecialMessage1>>().RouteInputs.
                Select(x => x.Name).ShouldHaveTheSameElementsAs("Id");
            graph.RouteFor<SpecialController>(x => x.Go3(null)).As<RouteDefinition<SpecialMessage2>>().RouteInputs.
                Select(x => x.Name).ShouldHaveTheSameElementsAs("Id");
            graph.RouteFor<SpecialController>(x => x.Go4(null)).As<RouteDefinition<SpecialMessage3>>().RouteInputs.
                Select(x => x.Name).ShouldHaveTheSameElementsAs("Id");
        }

        [Test]
        public void the_url_pattern_should_include_ExtraParam_for_NeedsId_action()
        {
            graph.RouteFor<SpecialController>(x => x.NeedsId(null)).As<RouteDefinition<SpecialRouteInputTestingMessage>>
                ().Pattern.ShouldEqual("special/needsid/{Id}/{ExtraParam}");
        }

        [Test]
        public void the_url_pattern_should_include_ExtraParam_for_NoId_action()
        {
            graph.RouteFor<SpecialController>(x => x.NoId(null)).As<RouteDefinition<SpecialRouteInputTestingMessage>>().
                Pattern.ShouldEqual("special/noid/{ExtraParam}");
        }

        [Test]
        public void the_url_pattern_should_include_id_for_NeedsId_action()
        {
            graph.RouteFor<SpecialController>(x => x.NeedsId(null)).As<RouteDefinition<SpecialRouteInputTestingMessage>>
                ().Pattern.ShouldEqual("special/needsid/{Id}/{ExtraParam}");
        }

        [Test]
        public void the_url_pattern_should_include_the_route_input()
        {
            graph.RouteFor<SpecialController>(x => x.Go1(null)).As<RouteDefinition<InputModel>>().Pattern.ShouldEqual(
                "special/go1");
            graph.RouteFor<SpecialController>(x => x.Go2(null)).As<RouteDefinition<SpecialMessage1>>().Pattern.
                ShouldEqual("special/go2/{Id}");
        }

        [Test]
        public void the_url_pattern_should_not_include_id_for_NoId_action()
        {
            graph.RouteFor<SpecialController>(x => x.NoId(null)).As<RouteDefinition<SpecialRouteInputTestingMessage>>().
                Pattern.ShouldEqual("special/noid/{ExtraParam}");
        }
    }

    [TestFixture]
    public class when_registering_home
    {
        private BehaviorGraph graphWithHome;
        private BehaviorGraph graphWithMethodHome;
        private BehaviorGraph graphWithoutHome;

        public class OneController
        {
            public void Home()
            {
            }

            public void HomeWithOutput(SimpleOutputModel model) {}
        }
        public class SimpleOutputModel {}

        [SetUp]
        public void SetUp()
        {
            graphWithHome = new FubuRegistry(x =>
                {
                    x.Actions.IncludeClassesSuffixedWithController();
                    x.HomeIs<OneController>(c => c.Home());
                }).BuildGraph();

            graphWithMethodHome = new FubuRegistry(x =>
                {
                    x.Actions.IncludeClassesSuffixedWithController();
                    x.HomeIs<SimpleOutputModel>();
                }).BuildGraph();

            graphWithoutHome = new FubuRegistry(x => x.Actions.IncludeClassesSuffixedWithController()).BuildGraph();
        }

        [Test]
        public void home_route_definition_pattern_should_be_empty()
        {
            var homeDefinition1 = graphWithHome.RouteFor<OneController>(c => c.Home());
            var homeDefinition2 = graphWithMethodHome.RouteFor<OneController>(c=>c.HomeWithOutput(new SimpleOutputModel()));
            var notHomeDefinition = graphWithoutHome.RouteFor<OneController>(c => c.Home());
            homeDefinition1.Pattern.ShouldEqual("");
            homeDefinition2.Pattern.ShouldEqual("");
            notHomeDefinition.Pattern.ShouldNotBeEmpty();
        }
    }

}