using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;

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
            graph.RouteFor<SpecialController>(x => x.Go1(null)).Input.As<RouteInput<InputModel>>().RouteParameters.Count.
                ShouldEqual(0);
            graph.RouteFor<SpecialController>(x => x.Go2(null)).Input.As<RouteInput<SpecialMessage1>>().RouteParameters.
                Select(x => x.Name).ShouldHaveTheSameElementsAs("Id");
            graph.RouteFor<SpecialController>(x => x.Go3(null)).Input.As<RouteInput<SpecialMessage2>>().RouteParameters.
                Select(x => x.Name).ShouldHaveTheSameElementsAs("Id");
            graph.RouteFor<SpecialController>(x => x.Go4(null)).Input.As<RouteInput<SpecialMessage3>>().RouteParameters.
                Select(x => x.Name).ShouldHaveTheSameElementsAs("Id");
        }

        [Test]
        public void the_url_pattern_should_include_ExtraParam_for_NeedsId_action()
        {
            graph.RouteFor<SpecialController>(x => x.NeedsId(null)).Pattern.ShouldEqual("special/needsid/{Id}/{ExtraParam}");
        }

        [Test]
        public void the_url_pattern_should_include_ExtraParam_for_NoId_action()
        {
            graph.RouteFor<SpecialController>(x => x.NoId(null)).
                Pattern.ShouldEqual("special/noid/{ExtraParam}");
        }

        [Test]
        public void the_url_pattern_should_include_id_for_NeedsId_action()
        {
            graph.RouteFor<SpecialController>(x => x.NeedsId(null)).Pattern.ShouldEqual("special/needsid/{Id}/{ExtraParam}");
        }

        [Test]
        public void the_url_pattern_should_include_the_route_input()
        {
            graph.RouteFor<SpecialController>(x => x.Go1(null)).Pattern.ShouldEqual(
                "special/go1");
            graph.RouteFor<SpecialController>(x => x.Go2(null)).Pattern.
                ShouldEqual("special/go2/{Id}");
        }

        [Test]
        public void the_url_pattern_should_not_include_id_for_NoId_action()
        {
            graph.RouteFor<SpecialController>(x => x.NoId(null)).
                Pattern.ShouldEqual("special/noid/{ExtraParam}");
        }
    }

    [TestFixture]
    public class when_registering_home
    {
        private BehaviorGraph graphWithMethodHome;
        private BehaviorGraph graphWithModelHome;
        private BehaviorGraph graphWithoutHome;
        private BehaviorGraph graphWithHomeAndUrlPolicy;

        public class OneController
        {
            public void Home() { }

            public SimpleOutputModel HomeWithInputOutput(SimpleInputModel model)
            {
                return new SimpleOutputModel();
            }
        }
        public class SimpleOutputModel {}
        public class SimpleInputModel {}
        public class AllEncompassingUrlPolicy : IUrlPolicy
        {
            public bool Matches(ActionCall call, IConfigurationObserver log)
            {
                return call.HandlerType.Name.EndsWith("Controller");
            }

            public IRouteDefinition Build(ActionCall call)
            {
                var route = call.ToRouteDefinition();
                route.Append(call.HandlerType.Name.RemoveSuffix("Controller"));
                route.Append(call.Method.Name);
                return route;
            }
        }

        [SetUp]
        public void SetUp()
        {
            graphWithMethodHome = new FubuRegistry(x =>
                {
                    x.Actions.IncludeClassesSuffixedWithController();
                    x.Routes.HomeIs<OneController>(c => c.Home());
                }).BuildGraph();

            graphWithModelHome = new FubuRegistry(x =>
                {
                    x.Actions.IncludeClassesSuffixedWithController();
                    x.Routes.HomeIs<SimpleInputModel>();
                }).BuildGraph();

            graphWithoutHome = new FubuRegistry(x => x.Actions.IncludeClassesSuffixedWithController()).BuildGraph();

            graphWithHomeAndUrlPolicy = new FubuRegistry(x =>
                {
                    x.Actions.IncludeClassesSuffixedWithController();

                    x.Routes
                        .UrlPolicy<AllEncompassingUrlPolicy>()
                        .HomeIs<SimpleInputModel>();
                }).BuildGraph();
        }

        [Test]
        public void home_route_definition_pattern_should_be_empty()
        {
            var homeDefinition1 = graphWithMethodHome.RouteFor<OneController>(c => c.Home());
            var homeDefinition2 = graphWithModelHome.RouteFor<OneController>(c => c.HomeWithInputOutput(new SimpleInputModel()));
            var notHomeDefinition = graphWithoutHome.RouteFor<OneController>(c => c.Home());
            homeDefinition1.Pattern.ShouldEqual("");
            homeDefinition2.Pattern.ShouldEqual("");
            notHomeDefinition.Pattern.ShouldNotBeEmpty();
        }

        [Test]
        public void home_url_policy_registration_should_be_higher_priority()
        {
            var homeDefinition = graphWithHomeAndUrlPolicy.RouteFor<OneController>(c => c.HomeWithInputOutput(new SimpleInputModel()));
            homeDefinition.Pattern.ShouldEqual("");
        }
    }

    public static class StringExtensions
    {
        public static string RemoveSuffix(this string value, string suffix)
        {
            if (value.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
                return value.Substring(0, value.Length - suffix.Length);
            return value;
        }
    }
}