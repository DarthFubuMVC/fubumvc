using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class RouteInputTester
    {
        private RouteParameter _parameter;
        public class FakeInput {public string Code { get; set; }}
        
        [SetUp]
        public void SetUp()
        {
            _parameter = new RouteParameter(ReflectionHelper.GetAccessor<FakeInput>(x => x.Code));
        }

        [Test]
        public void to_query_string_of_empty_value_returns_name()
        {
            _parameter.ToQueryString(new FakeInput()).ShouldBe(_parameter.Name + "=");
        }

        [Test]
        public void to_query_string_fromwith_encoded_value()
        {
            _parameter.ToQueryString(new FakeInput { Code = "abc/def&ghi=jkl" }).ShouldBe(_parameter.Name + "=abc%2Fdef%26ghi%3Djkl");
        }

        [Test]
        public void substitute_on_route_parameter()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something";
            _parameter.Substitute(parameters, "aaa/{Code}/aaa").ShouldBe("aaa/something/aaa");
        }

        [Test]
        public void substitute_on_route_parameter_deals_with_html_encoding()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something&else";
            _parameter.Substitute(parameters, "aaa/{Code}/aaa").ShouldBe("aaa/something%26else/aaa");
        }

        [Test]
        public void can_substitute_greedy_parameters()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "computers/laptop/i7/dell&ibm";
            _parameter.Substitute(parameters, "product/category/{*Code}").ShouldBe("product/category/computers/laptop/i7/dell%26ibm");
        }

        [Test]
        public void can_substitute_greedy_parameters_deals_with_html_encoding()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "laptop&apple";
            _parameter.Substitute(parameters, "product/category/{*Code}").ShouldBe("product/category/laptop%26apple");
        }

        [Test]
        public void is_satisfied_negative()
        {
            _parameter.IsSatisfied(new RouteParameters()).ShouldBeFalse();
        }

        [Test]
        public void is_satisfied_positive()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something";

            _parameter.IsSatisfied(parameters).ShouldBeTrue();
        }

        [Test]
        public void is_satisfied_with_a_default_value_but_nothing_in_the_parameters()
        {
            var parameters = new RouteParameters<FakeInput>();

            _parameter.DefaultValue = "something";

            _parameter.IsSatisfied(parameters).ShouldBeTrue();
        }

        [Test]
        public void to_query_string_from_route_parameter_with_no_values()
        {
            _parameter.ToQueryString(new RouteParameters()).ShouldBe("Code=");
        }

        [Test]
        public void to_query_string_from_route_parameter_with_a_value()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something";

            _parameter.ToQueryString(parameters).ShouldBe("Code=something");
        }

        [Test]
        public void to_query_string_from_route_parameter_with_an_encoded_value()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "abc/def&ghi=jkl";

            _parameter.ToQueryString(parameters).ShouldBe("Code=abc%2Fdef%26ghi%3Djkl");
        }
    }
}