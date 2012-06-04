using System.Linq;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Registration.Conventions.Handlers;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class HandlersUrlPolicyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void before_each()
        {
            _policy = new HandlersUrlPolicy(typeof (HandlersMarker));
        }

        #endregion

        private HandlersUrlPolicy _policy;

        [Test]
        public void should_add_querystrings_to_route_for_handler_convention()
        {
            _policy
                .Build(HandlersObjectMother.HandlerCall())
                .Input.QueryParameters.First().Name.ShouldEqual("Input");
        }

        [Test]
        public void should_apply_input_types_for_verb_handler_convention()
        {
            var input = _policy.Build(HandlersObjectMother.VerbHandler()).Input;

            input.QueryParameters.First().Name.ShouldEqual("Optional");
        }

        [Test]
        public void should_constrain_routes_by_MethodToUrlBuilder_if_they_match()
        {
            var input = _policy.Build(HandlersObjectMother.HandlerWithRouteInput()).Input;
            var parameters = new RouteParameters();
            parameters["Year"] = "2011";
            parameters["Month"] = "7";
            parameters["Title"] = "hello-world";

            input
                .CreateUrlFromParameters(parameters)
                .ShouldEqual("posts/2011/7/hello-world");
        }

        [Test]
        public void should_constrain_routes_by_class_name_without_handler()
        {
            _policy
                .Build(HandlersObjectMother.HandlerCall())
                .AllowedHttpMethods
                .ShouldContain("GET");
        }

        [Test]
        public void should_not_match_calls_with_url_pattern_attribute()
        {
            var log = new NulloConfigurationObserver();
            _policy
                .Matches(HandlersObjectMother.HandlerWithAttributeCall(), log)
                .ShouldBeFalse();
        }

        [Test]
        public void should_only_match_calls_with_handler_type_ending_with_handler()
        {
            var log = new NulloConfigurationObserver();
            _policy
                .Matches(HandlersObjectMother.HandlerCall(), log)
                .ShouldBeTrue();
            _policy
                .Matches(HandlersObjectMother.NonHandlerCall(), log)
                .ShouldBeFalse();
        }

        [Test]
        public void should_strip_root_namespace_and_treat_child_namespaces_as_folders()
        {
            _policy
                .Build(HandlersObjectMother.HandlerCall())
                .Pattern
                .ShouldEqual("posts/create");
        }

        [Test]
        public void should_use_hyphen_to_break_up_camel_casing()
        {
            _policy
                .Build(HandlersObjectMother.ComplexHandlerCall())
                .Pattern
                .ShouldEqual("posts/complex-route");
        }
    }
}