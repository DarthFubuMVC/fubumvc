using FubuCore.Reflection;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class RouteInputTester
    {
        private RouteInput _input;
        public class FakeInput {public string Code { get; set; }}
        
        [SetUp]
        public void SetUp()
        {
            _input = new RouteInput(ReflectionHelper.GetAccessor<FakeInput>(x => x.Code));
        }

        [Test]
        public void to_query_string_of_empty_value_returns_name()
        {
            _input.ToQueryString(new FakeInput()).ShouldEqual(_input.Name + "=");
        }

        [Test]
        public void substitute_on_route_parameter()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something";
            _input.Substitute(parameters, "aaa/{Code}/aaa").ShouldEqual("aaa/something/aaa");
        }

        [Test]
        public void substitute_on_route_parameter_deals_with_html_encoding()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something&else";
            _input.Substitute(parameters, "aaa/{Code}/aaa").ShouldEqual("aaa/something%26else/aaa");
        }

        [Test]
        public void is_satisfied_negative()
        {
            _input.IsSatisfied(new RouteParameters()).ShouldBeFalse();
        }

        [Test]
        public void is_satisfied_positive()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something";

            _input.IsSatisfied(parameters).ShouldBeTrue();
        }

        [Test]
        public void is_satisfied_with_a_default_value_but_nothing_in_the_parameters()
        {
            var parameters = new RouteParameters<FakeInput>();

            _input.DefaultValue = "something";

            _input.IsSatisfied(parameters).ShouldBeTrue();
        }

        [Test]
        public void to_query_string_from_route_parameter_with_no_values()
        {
            _input.ToQueryString(new RouteParameters()).ShouldEqual("Code=");
        }

        [Test]
        public void to_query_string_from_route_parameter_with_a_value()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something";

            _input.ToQueryString(parameters).ShouldEqual("Code=something");
        }
    }
}