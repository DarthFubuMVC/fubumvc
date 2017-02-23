using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Registration
{
    
    public class RouteInputTester
    {
        private RouteParameter _parameter = new RouteParameter(ReflectionHelper.GetAccessor<FakeInput>(x => x.Code));
        public class FakeInput {public string Code { get; set; }}
        
        [Fact]
        public void to_query_string_of_empty_value_returns_name()
        {
            _parameter.ToQueryString(new FakeInput()).ShouldBe(_parameter.Name + "=");
        }

        [Fact]
        public void to_query_string_fromwith_encoded_value()
        {
            _parameter.ToQueryString(new FakeInput { Code = "abc/def&ghi=jkl" }).ShouldBe(_parameter.Name + "=abc%2Fdef%26ghi%3Djkl");
        }

        [Fact]
        public void substitute_on_route_parameter()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something";
            _parameter.Substitute(parameters, "aaa/{Code}/aaa").ShouldBe("aaa/something/aaa");
        }

        [Fact]
        public void substitute_on_route_parameter_deals_with_html_encoding()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something&else";
            _parameter.Substitute(parameters, "aaa/{Code}/aaa").ShouldBe("aaa/something%26else/aaa");
        }

        [Fact]
        public void can_substitute_greedy_parameters()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "computers/laptop/i7/dell&ibm";
            _parameter.Substitute(parameters, "product/category/{*Code}").ShouldBe("product/category/computers/laptop/i7/dell%26ibm");
        }

        [Fact]
        public void can_substitute_greedy_parameters_deals_with_html_encoding()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "laptop&apple";
            _parameter.Substitute(parameters, "product/category/{*Code}").ShouldBe("product/category/laptop%26apple");
        }

        [Fact]
        public void is_satisfied_negative()
        {
            _parameter.IsSatisfied(new RouteParameters()).ShouldBeFalse();
        }

        [Fact]
        public void is_satisfied_positive()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something";

            _parameter.IsSatisfied(parameters).ShouldBeTrue();
        }

        [Fact]
        public void is_satisfied_with_a_default_value_but_nothing_in_the_parameters()
        {
            var parameters = new RouteParameters<FakeInput>();

            _parameter.DefaultValue = "something";

            _parameter.IsSatisfied(parameters).ShouldBeTrue();
        }

        [Fact]
        public void to_query_string_from_route_parameter_with_no_values()
        {
            _parameter.ToQueryString(new RouteParameters()).ShouldBe("Code=");
        }

        [Fact]
        public void to_query_string_from_route_parameter_with_a_value()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "something";

            _parameter.ToQueryString(parameters).ShouldBe("Code=something");
        }

        [Fact]
        public void to_query_string_from_route_parameter_with_an_encoded_value()
        {
            var parameters = new RouteParameters<FakeInput>();
            parameters[x => x.Code] = "abc/def&ghi=jkl";

            _parameter.ToQueryString(parameters).ShouldBe("Code=abc%2Fdef%26ghi%3Djkl");
        }
    }
}