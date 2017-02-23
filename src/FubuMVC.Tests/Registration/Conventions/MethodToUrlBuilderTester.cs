using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Registration.Conventions
{
    
    public class MethodToUrlBuilderIntegratedTester
    {
        public MethodToUrlBuilderIntegratedTester()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<MethodAction>();
            });

            theChain = graph.ChainFor<MethodAction>(x => x.Get_cases_from_Start_to_End(null))
                .As<RoutedChain>();
        }


        private RoutedChain theChain;

        [Fact]
        public void the_chain_has_the_http_method_constraint()
        {
            theChain.Route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Fact]
        public void the_chain_has_the_url_pattern()
        {
            theChain.Route.Pattern.ShouldBe("cases/from/{Start}/to/{End}");
        }

        [Fact]
        public void the_route_has_route_input()
        {
            var input = theChain.Route.Input.ShouldBeOfType<RouteInput<MethodInput>>();
            input.ShouldNotBeNull();

            var parameters = new RouteParameters();
            parameters["Start"] = "2";
            parameters["End"] = "5";

            input.CreateUrlFromParameters(parameters).ShouldBe("cases/from/2/to/5");
        }
    }

    public class MethodAction
    {
        public string Get_cases_from_Start_to_End(MethodInput input)
        {
            return "stuff";
        }
    }

    public class MethodInput
    {
        public int Start { get; set; }
        public int End { get; set; }
    }


    
    public class MethodToUrlBuilderTester
    {
        private List<string> theProperties = new List<string>();
        private RouteDefinition theRoute = new RouteDefinition("");

        [Fact]
        public void allow_get_as_a_prefix()
        {
            MethodToUrlBuilder.Alter(theRoute, "GetPath", theProperties);

            theRoute.Pattern.ShouldBe("getpath");
            theRoute.AllowedHttpMethods.Any().ShouldBeFalse();
        }

        [Fact]
        public void create_http_constraint_for_get()
        {
            MethodToUrlBuilder.Alter(theRoute, "Get_path", theProperties);

            theRoute.Pattern.ShouldBe("path");
            theRoute.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Fact]
        public void create_http_constraint_for_post()
        {
            MethodToUrlBuilder.Alter(theRoute, "post_path", theProperties);

            theRoute.Pattern.ShouldBe("path");
            theRoute.AllowedHttpMethods.ShouldHaveTheSameElementsAs("POST");
        }

        [Fact]
        public void create_http_constraint_for_put()
        {
            MethodToUrlBuilder.Alter(theRoute, "Put_path", theProperties);

            theRoute.Pattern.ShouldBe("path");
            theRoute.AllowedHttpMethods.ShouldHaveTheSameElementsAs("PUT");
        }

        [Fact]
        public void http_verb_only_should_not_modify_route()
        {
            const string originalRoute = "base/route";
            theRoute.Append(originalRoute);

            MethodToUrlBuilder.Alter(theRoute, "get", theProperties);

            theRoute.Pattern.ShouldBe(originalRoute);
            theRoute.AllowedHttpMethods.ShouldContain("GET");
        }

        [Fact]
        public void matches_negative()
        {
            MethodToUrlBuilder.Matches("somethingelse").ShouldBeFalse();
        }

        [Fact]
        public void matches_positive()
        {
            MethodToUrlBuilder.Matches("get_this").ShouldBeTrue();
        }

        [Fact]
        public void multiple_substitutions()
        {
            theProperties.Add("Start");
            theProperties.Add("End");

            MethodToUrlBuilder.Alter(theRoute, "cases_from_Start_to_End", theProperties);

            theRoute.Pattern.ShouldBe("cases/from/{Start}/to/{End}");
        }

        [Fact]
        public void multiple_substitutions_and_folders_and_constraint()
        {
            theProperties.Add("Start");
            theProperties.Add("End");

            MethodToUrlBuilder.Alter(theRoute, "get_cases_from_Start_to_End", theProperties);

            theRoute.Pattern.ShouldBe("cases/from/{Start}/to/{End}");
            theRoute.AllowedHttpMethods.ShouldContain("GET");
        }

        [Fact]
        public void use_separator_and_substitution_for_matching_property()
        {
            theProperties.Add("Input1");
            MethodToUrlBuilder.Alter(theRoute, "path_Input1_folder", theProperties);

            theRoute.Pattern.ShouldBe("path/{Input1}/folder");
        }

        [Fact]
        public void use_separators_for_underscores_if_not_a_route_input()
        {
            MethodToUrlBuilder.Alter(theRoute, "path_folder1_folder2", theProperties);

            theRoute.Pattern.ShouldBe("path/folder1/folder2");
        }

        [Fact]
        public void two_underscores_in_a_row_are_considered_to_be_an_underscore()
        {
            MethodToUrlBuilder.Alter(theRoute, "path_folder1__folder2", theProperties);
            theRoute.Pattern.ShouldBe("path/folder1/_folder2");
        }

        [Fact]
        public void three_underscores_in_a_row_are_a_dash()
        {
            MethodToUrlBuilder.Alter(theRoute, "path_folder1___folder2", theProperties);
            theRoute.Pattern.ShouldBe("path/folder1-folder2");
        }
    }
}