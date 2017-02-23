using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Routing;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using Xunit;
using FubuMVC.Tests;

namespace FubuMVC.Tests.Registration
{
    
    public class RouteDefinitionTester
    {
        public class QueryStringTarget
        {
            [QueryString]
            public string Name { get; set; }

            [QueryString]
            public string Age { get; set; }
        }

        public class SampleViewModel
        {
            public string InPath { get; set; }
            public string AlsoInPath { get; set; }
            public string InQueryString { get; set; }
            public string AlsoInQueryString { get; set; }
        }

        public class SampleViewModelWithInputs
        {
            [RouteInput]
            public string RequiredInput { get; set; }

            [RouteInput("default")]
            public string OptionalInput { get; set; }
        }

        public class MyOwnUrlMaker : IMakeMyOwnUrl
        {
            private readonly string _part;

            public MyOwnUrlMaker(string part)
            {
                _part = part;
            }

            public string ToUrlPart(string basePattern)
            {
                return basePattern + "/" + _part;
            }
        }

        [Fact]
        public void can_calculate_rank_without_input_if_there_are_substitutions()
        {
            var route = new RouteDefinition("{Client}/foo");
            route.Rank.ShouldBe(1);
        }

        [Fact]
        public void session_state_requirement_is_null_by_default_signifying_that_it_should_use_the_default()
        {
            new RouteDefinition("something").SessionStateRequirement.ShouldBeNull(); // this actually matters
        }

        [Fact]
        public void get_route_pattern_with_querystring_parameters_when_there_are_no_values()
        {
            var route = RouteBuilder.Build(typeof (QueryStringTarget), "route");

            route.CreateUrlFromInput(new QueryStringTarget())
                .ShouldBe("route");
        }

        [Fact]
        public void get_route_pattern_with_an_IMakeMyOwnUrl()
        {
            var maker = new MyOwnUrlMaker("something/else");
            var route = new RouteDefinition("folder");

            route.CreateUrlFromInput(maker).ShouldBe("folder/something/else");
        }

        [Fact]
        public void by_default_input_type_returns_null()
        {
            var routeDefinition = new RouteDefinition("");
            routeDefinition.Input.ShouldBeNull();
        }

        [Fact]
        public void to_string_by_default_gets_pattern()
        {
            const string pattern = "some/pattern";
            var routeDefinition = new RouteDefinition(pattern);
            routeDefinition.ToString().ShouldBe(pattern);
        }

        [Fact]
        public void shouldd_not_add_duplicate_input()
        {
            var url = new RouteInput<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(new RouteParameter(ReflectionHelper.GetAccessor<SampleViewModel>(x=>x.InPath)), false);
            url.RouteParameters.ShouldHaveCount(1);
        }

        [Fact]
        public void to_string_gets_pattern_and_type_full_name()
        {
            var url = new RouteInput<SampleViewModel>("my/sample");
            url.ToString().ShouldBe("{0} --> {1}".ToFormat(url.Parent.Pattern, typeof(SampleViewModel).FullName));
        }

        [Fact]
        public void create_default_value_for_a_route()
        {
            var url = new RouteInput<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            url.RouteInputFor("InPath").DefaultValue = "something";
            Route route = url.Parent.ToRoute();

            route.Defaults["InPath"].ShouldBe("something");
        }

        [Fact]
        public void create_two_default_values_for_a_route()
        {
            var url = new RouteInput<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);
            url.RouteInputFor("InPath").DefaultValue = "something";
            url.RouteInputFor("AlsoInPath").DefaultValue = "something else";
            Route route = url.Parent.ToRoute();

            route.Defaults.Count().ShouldBe(2);
        }


        [Fact]
        public void create_url_for_a_route_with_two_parameters()
        {
            var url = new RouteInput<SampleViewModel>("my/sample/{InPath}/{AlsoInPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);

            url.Parent.ToRoute().Url.ShouldBe("my/sample/{InPath}/{AlsoInPath}");
        }

        [Fact]
        public void create_url_will_escape_the_url()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);

            url.CreateUrlFromInput(new SampleViewModel
            {
                InPath = "some text"
            }).ShouldBe("test/edit/some%20text");
        }

        [Fact]
        public void create_url_will_escape_the_url_with_parameters()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);

            var parameters = new RouteParameters<SampleViewModel>();
            parameters[x => x.InPath] = "some text";

            url.CreateUrlFromParameters(parameters).ShouldBe("test/edit/some%20text");
        }

        [Fact]
        public void create_url_with_input_model()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);

            url.CreateUrlFromInput(new SampleViewModel
            {
                InPath = "5"
            }).ShouldBe("test/edit/5");
        }

        [Fact]
        public void create_url_with_parameters()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);

            var parameters = new RouteParameters<SampleViewModel>();
            parameters[x => x.InPath] = "5";

            url.CreateUrlFromParameters(parameters).ShouldBe("test/edit/5");
        }

        [Fact]
        public void create_url_with_input_model_and_encoded_variable()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);

            url.CreateUrlFromInput(new SampleViewModel
            {
                InPath = "abc/def&ghi=jkl"
            }).ShouldBe("test/edit/abc%2Fdef%26ghi%3Djkl");
        }

        [Fact]
        public void create_url_with_input_model_and_default_value_for_optional_input()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{OptionalInput}");
            url.AddRouteInput(x => x.OptionalInput);

            url
                .CreateUrlFromInput(new SampleViewModelWithInputs
                {
                    OptionalInput = "a"
                })
                .ShouldEndWith("test/edit/a");
        }

        [Fact]
        public void create_url_with_input_model_and_default_value_for_optional_input_with_parameters()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{OptionalInput}");
            url.AddRouteInput(x => x.OptionalInput);

            var parameters = new RouteParameters<SampleViewModelWithInputs>();
            parameters[x => x.OptionalInput] = "a";

            url
                .CreateUrlFromParameters(parameters)
                .ShouldEndWith("test/edit/a");
        }

        [Fact]
        public void create_url_with_input_model_and_default_value_for_required_input()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{RequiredInput}");
            url.AddRouteInput(x => x.RequiredInput);

            url
                .CreateUrlFromInput(new SampleViewModelWithInputs
                {
                    RequiredInput = "a"
                })
                .ShouldEndWith("test/edit/a");
        }

        [Fact]
        public void create_url_with_input_model_and_default_value_for_required_input_by_parameters()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{RequiredInput}");
            url.AddRouteInput(x => x.RequiredInput);

            var parameters = new RouteParameters<SampleViewModelWithInputs>();
            parameters[x => x.RequiredInput] = "a";

            url
                .CreateUrlFromParameters(parameters)
                .ShouldEndWith("test/edit/a");
        }

        [Fact]
        public void create_url_with_input_model_and_no_default_value_specified_for_optional_input()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{OptionalInput}");
            url.AddRouteInput(x => x.OptionalInput);

            url
                .CreateUrlFromInput(new SampleViewModelWithInputs())
                .ShouldEndWith("test/edit/default");
        }


        [Fact]
        public void create_url_with_input_model_and_no_default_value_specified_for_optional_input_by_parameters()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{OptionalInput}");
            url.AddRouteInput(x => x.OptionalInput);

            var parameters = new RouteParameters<SampleViewModelWithInputs>();

            url
                .CreateUrlFromParameters(parameters)
                .ShouldEndWith("test/edit/default");
        }

        [Fact]
        public void create_url_with_input_model_and_no_default_value_specified_for_required_input()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{RequiredInput}");
            url.AddRouteInput(x => x.RequiredInput);

            Exception<FubuException>.ShouldBeThrownBy(() => url.CreateUrlFromInput(new SampleViewModelWithInputs()));
        }

        [Fact]
        public void create_url_with_input_model_and_no_default_value_specified_for_required_input_by_parameters()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{RequiredInput}");
            url.AddRouteInput(x => x.RequiredInput);

            Exception<FubuException>.ShouldBeThrownBy(() => url.CreateUrlFromParameters(new RouteParameters()));
        }

        [Fact]
        public void create_url_with_multiple_variables_in_path()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}/{AlsoInPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);

            url.CreateUrlFromInput(new SampleViewModel
            {
                InPath = "5",
                AlsoInPath = "some text"
            }).ShouldBe("test/edit/5/some%20text");
        }

        [Fact]
        public void create_url_with_multiple_variables_in_path_by_parameters()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}/{AlsoInPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);

            var parameters = new RouteParameters<SampleViewModel>();
            parameters[x => x.InPath] = "5";
            parameters[x => x.AlsoInPath] = "some text";

            url.CreateUrlFromParameters(parameters).ShouldBe("test/edit/5/some%20text");
        }

        [Fact]
        public void create_url_with_encoded_variables_in_path_by_parameters()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}/{AlsoInPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);

            var parameters = new RouteParameters<SampleViewModel>();
            parameters[x => x.InPath] = "5";
            parameters[x => x.AlsoInPath] = "abc/def&ghi=jkl";

            url.CreateUrlFromParameters(parameters).ShouldBe("test/edit/5/abc%2Fdef%26ghi%3Djkl");
        }

        [Fact]
        public void create_url_with_multiple_variables_in_querystring()
        {
            var url = new RouteInput<SampleViewModel>("/my/sample/path");
            var props = new List<Expression<Func<SampleViewModel, object>>>
                              {
                                  x => x.InQueryString,
                                  x => x.AlsoInQueryString
                              };
            var inputs = props.Select(x => new RouteParameter(ReflectionHelper.GetAccessor(x)));
            url.AddQueryInputs(inputs);

            url.CreateUrlFromInput(new SampleViewModel
            {
                InQueryString = "query",
                AlsoInQueryString = "alsoquery"
            })
                .ShouldBe("/my/sample/path?InQueryString=query&AlsoInQueryString=alsoquery");
        }

        [Fact]
        public void create_url_with_null_input_model_and_no_default_value_specified_for_optional_input()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{OptionalInput}");
            url.AddRouteInput(x => x.OptionalInput);

            url
                .CreateUrlFromInput(null)
                .ShouldEndWith("test/edit/default");
        }


        [Fact]
        public void create_url_with_null_input_model_and_no_default_value_specified_for_optional_input_with_parameters()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{OptionalInput}");
            url.AddRouteInput(x => x.OptionalInput);

            var parameters = new RouteParameters<SampleViewModelWithInputs>();

            url
                .CreateUrlFromParameters(parameters)
                .ShouldEndWith("test/edit/default");
        }

        [Fact]
        public void create_url_with_null_input_model_and_no_default_value_specified_for_required_input()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{RequiredInput}");
            url.AddRouteInput(x => x.RequiredInput);

            Exception<FubuException>.ShouldBeThrownBy(() => url.CreateUrlFromInput(null));
        }

        [Fact]
        public void create_url_with_null_input_model_and_no_default_value_specified_for_required_input_with_parameters()
        {
            var url = new RouteInput<SampleViewModelWithInputs>("test/edit/{RequiredInput}");
            url.AddRouteInput(x => x.RequiredInput);

            Exception<FubuException>.ShouldBeThrownBy(() => url.CreateUrlFromParameters(null));
        }

        [Fact]
        public void create_url_with_querystring_and_inputmodel()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddQueryInput(x => x.InQueryString);

            url.CreateUrlFromInput(new SampleViewModel
            {
                InPath = "5",
                InQueryString = "query"
            }).ShouldBe("test/edit/5?InQueryString=query");
        }

        [Fact]
        public void will_not_use_query_string_if_there_is_no_value()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddQueryInput(x => x.InQueryString);

            url.CreateUrlFromInput(new SampleViewModel
            {
                InPath = "5",
                InQueryString = null
            }).ShouldBe("test/edit/5");
        }

        [Fact]
        public void create_url_with_querystring_and_inputmodel_with_parameters()
        {
            var url = new RouteInput<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddQueryInput(x => x.InQueryString);

            var parameters = new RouteParameters<SampleViewModel>();
            parameters[x => x.InPath] = "5";
            parameters[x => x.InQueryString] = "query";

            url.CreateUrlFromParameters(parameters).ShouldBe("test/edit/5?InQueryString=query");
        }

        [Fact]
        public void create_url_with_variables_in_querystring()
        {
            var url = new RouteInput<SampleViewModel>("/my/sample/path");
            url.AddQueryInput(x => x.InQueryString);

            url.CreateUrlFromInput(new SampleViewModel
            {
                InQueryString = "query"
            }).ShouldBe("/my/sample/path?InQueryString=query");
        }


        [Fact]
        public void create_url_with_variables_in_querystring_with_parameters()
        {
            var url = new RouteInput<SampleViewModel>("/my/sample/path");
            url.AddQueryInput(x => x.InQueryString);

            var parameters = new RouteParameters<SampleViewModel>();
            parameters[x => x.InQueryString] = "query";

            url.CreateUrlFromParameters(parameters).ShouldBe("/my/sample/path?InQueryString=query");
        }

        [Fact]
        public void does_not_create_default_for_a_simple_parameter()
        {
            var url = new RouteInput<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            Route route = url.Parent.ToRoute();

            route.Defaults.Count().ShouldBe(0);
        }

        [Fact]
        public void prepend_to_route()
        {
            var route = new RouteDefinition("my/sample");
            route.Prepend("area");

            route.Pattern.ShouldBe("area/my/sample");
        }

        [Fact]
        public void prepend_to_route_takes_care_of_stupid_trailing_leading_slashes()
        {
            var route = new RouteDefinition("my/sample");
            route.Prepend("area/");

            route.Pattern.ShouldBe("area/my/sample");
        }

        [Fact]
        public void route_input_should_substitute_method()
        {
            SingleProperty accessor = SingleProperty.Build<SampleViewModel>(x => x.InPath);
            var viewModel = new SampleViewModel
            {
                InPath = "5"
            };
            var routeInput = new RouteParameter(accessor);

            routeInput.Substitute(viewModel, "test/edit/{InPath}").ShouldBe("test/edit/5");
        }

        [Fact]
        public void should_have_one_default_value_for_a_route_and_does_not_include_querystring_in_route()
        {
            var url = new RouteInput<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            url.AddQueryInput(x => x.InQueryString);
            url.RouteInputFor("InPath").DefaultValue = "something";
            url.QueryInputFor("InQueryString").DefaultValue = "querysomething";
            Route route = url.Parent.ToRoute();

            route.Defaults.Count().ShouldBe(1);
        }

        [Fact]
        public void should_have_one_default_value_for_route()
        {
            var url = new RouteInput<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);
            url.RouteInputFor("InPath").DefaultValue = "something";
            Route route = url.Parent.ToRoute();

            route.Defaults.Count().ShouldBe(1);
        }

        [Fact]
        public void should_not_have_constraints_by_default()
        {
            var url = new RouteDefinition("my/sample");
            Route route = url.ToRoute();

            route.Constraints.ShouldBeNull();
        }


        [Fact]
        public void no_http_method_constraints_on_startup()
        {
            var route = new RouteDefinition("something");
            route.GetHttpMethodConstraints().Any().ShouldBeFalse();
        }

        [Fact]
        public void add_http_method_constraint()
        {
            var route = new RouteDefinition("something");
            route.AddHttpMethodConstraint("get");

            route.GetHttpMethodConstraints().ShouldHaveTheSameElementsAs("GET");
        }

        [Fact]
        public void add_http_method_constraint_multiple_times()
        {
            var route = new RouteDefinition("something");
            route.AddHttpMethodConstraint("get");
            route.AddHttpMethodConstraint("Get");

            route.GetHttpMethodConstraints().ShouldHaveTheSameElementsAs("GET");
        }

        [Fact]
        public void add_multiple_http_constraints()
        {
            var route = new RouteDefinition("something");
            route.AddHttpMethodConstraint("get");
            route.AddHttpMethodConstraint("POST");

            route.GetHttpMethodConstraints().ShouldHaveTheSameElementsAs("GET", "POST");
        }

        [Fact]
        public void add_multiple_http_method_constraint_multiple_times()
        {
            var route = new RouteDefinition("something");
            route.AddHttpMethodConstraint("get");
            route.AddHttpMethodConstraint("Get");
            route.AddHttpMethodConstraint("Post");
            route.AddHttpMethodConstraint("POST");

            route.GetHttpMethodConstraints().ShouldHaveTheSameElementsAs("GET", "POST");
        }

        [Fact]
        public void create_route_with_no_http_constraints()
        {
            var route = new RouteDefinition("something").ToRoute();
            route.Constraints.ShouldBeNull();
        }

        [Fact]
        public void create_route_with_http_constraints()
        {
            var route = new RouteDefinition("something");
            route.AddHttpMethodConstraint("Get");
            route.AddHttpMethodConstraint("POST");

        
            route.ToRoute().Constraints.Single().Value.ShouldBeOfType<HttpMethodConstraint>()
                .AllowedMethods.ShouldHaveTheSameElementsAs("GET", "POST");
        }

        [Fact]
        public void responds_to_get_with_no_constraints()
        {
            var route = new RouteDefinition("something");
            route.RespondsToMethod("GET").ShouldBeTrue();
        }

        [Fact]
        public void responds_to_get_if_GET_is_explicitly_allowed()
        {
            var route = new RouteDefinition("something");
            route.AddHttpMethodConstraint("Get");
            route.AddHttpMethodConstraint("POST");
            route.RespondsToMethod("GET").ShouldBeTrue();
        }

        [Fact]
        public void does_not_respond_to_get_if_http_methods_are_explicitly_defined_and_get_is_not_allowed()
        {
            var route = new RouteDefinition("something");
            route.AddHttpMethodConstraint("POST");
            route.RespondsToMethod("GET").ShouldBeFalse();
        }

        [Fact]
        public void default_value_syntax_for_urlpattern_produces_correct_route()
        {
            var parent = new RouteDefinition("my/sample/{InPath:hello}/{AlsoInPath:world}");
            parent.Input = new RouteInput<SampleViewModel>(parent);

            var route = parent.ToRoute();

            Regex.Match(route.Url, @":\w+").Success.ShouldBeFalse();
        }

        [Fact]
        public void describing_itself_session_state()
        {
            var route = new RouteDefinition("something");
            route.SessionStateRequirement.ShouldBeNull();

            Description.For(route).Properties["SessionStateRequirement"].ShouldBe("Default");


            route.SessionStateRequirement = SessionStateRequirement.RequiresSessionState;

            Description.For(route).Properties["SessionStateRequirement"].ShouldBe(
                SessionStateRequirement.RequiresSessionState.ToString());
        }

        [Fact]
        public void describing_itself_gets_the_title()
        {
            Description.For(new RouteDefinition("something")).Title.ShouldBe("Route:  something");
        }

        [Fact]
        public void describe_itself_adds_child_for_RouteInput()
        {
            var route = RouteBuilder.Build(typeof (BigModel), "big/{Name}/{State}");

            var description = Description.For(route);

            description.Children["Input"].Title.ShouldBe(Description.For(route.Input).Title);
        }

    }

    public class BigModel
    {
        public string Name { get; set; }
        public string State { get; set; }
    }
}