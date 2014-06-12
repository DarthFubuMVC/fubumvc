using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class MethodToUrlBuilderIntegratedTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<MethodAction>();
            });

            theChain = graph.BehaviorFor<MethodAction>(x => x.Get_cases_from_Start_to_End(null))
                .As<RoutedChain>();
        }

        #endregion

        private RoutedChain theChain;

        [Test]
        public void the_chain_has_the_http_method_constraint()
        {
            theChain.Route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Test]
        public void the_chain_has_the_url_pattern()
        {
            theChain.Route.Pattern.ShouldEqual("cases/from/{Start}/to/{End}");
        }

        [Test]
        public void the_route_has_route_input()
        {
            var input = theChain.Route.Input.ShouldBeOfType<RouteInput<MethodInput>>();
            input.ShouldNotBeNull();

            var parameters = new RouteParameters();
            parameters["Start"] = "2";
            parameters["End"] = "5";

            input.CreateUrlFromParameters(parameters).ShouldEqual("cases/from/2/to/5");
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


    [TestFixture]
    public class MethodToUrlBuilderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            theProperties = new List<string>();
            theRoute = new RouteDefinition("");
        }

        #endregion

        private List<string> theProperties;
        private RouteDefinition theRoute;

        [Test]
        public void allow_get_as_a_prefix()
        {
            MethodToUrlBuilder.Alter(theRoute, "GetPath", theProperties);

            theRoute.Pattern.ShouldEqual("getpath");
            theRoute.AllowedHttpMethods.Any().ShouldBeFalse();
        }

        [Test]
        public void create_http_constraint_for_get()
        {
            MethodToUrlBuilder.Alter(theRoute, "Get_path", theProperties);

            theRoute.Pattern.ShouldEqual("path");
            theRoute.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Test]
        public void create_http_constraint_for_post()
        {
            MethodToUrlBuilder.Alter(theRoute, "post_path", theProperties);

            theRoute.Pattern.ShouldEqual("path");
            theRoute.AllowedHttpMethods.ShouldHaveTheSameElementsAs("POST");
        }

        [Test]
        public void create_http_constraint_for_put()
        {
            MethodToUrlBuilder.Alter(theRoute, "Put_path", theProperties);

            theRoute.Pattern.ShouldEqual("path");
            theRoute.AllowedHttpMethods.ShouldHaveTheSameElementsAs("PUT");
        }

        [Test]
        public void http_verb_only_should_not_modify_route()
        {
            const string originalRoute = "base/route";
            theRoute.Append(originalRoute);

            MethodToUrlBuilder.Alter(theRoute, "get", theProperties);

            theRoute.Pattern.ShouldEqual(originalRoute);
            theRoute.AllowedHttpMethods.ShouldContain("GET");
        }

        [Test]
        public void matches_negative()
        {
            MethodToUrlBuilder.Matches("somethingelse").ShouldBeFalse();
        }

        [Test]
        public void matches_positive()
        {
            MethodToUrlBuilder.Matches("get_this").ShouldBeTrue();
        }

        [Test]
        public void multiple_substitutions()
        {
            theProperties.Add("Start");
            theProperties.Add("End");

            MethodToUrlBuilder.Alter(theRoute, "cases_from_Start_to_End", theProperties);

            theRoute.Pattern.ShouldEqual("cases/from/{Start}/to/{End}");
        }

        [Test]
        public void multiple_substitutions_and_folders_and_constraint()
        {
            theProperties.Add("Start");
            theProperties.Add("End");

            MethodToUrlBuilder.Alter(theRoute, "get_cases_from_Start_to_End", theProperties);

            theRoute.Pattern.ShouldEqual("cases/from/{Start}/to/{End}");
            theRoute.AllowedHttpMethods.ShouldContain("GET");
        }

        [Test]
        public void use_separator_and_substitution_for_matching_property()
        {
            theProperties.Add("Input1");
            MethodToUrlBuilder.Alter(theRoute, "path_Input1_folder", theProperties);

            theRoute.Pattern.ShouldEqual("path/{Input1}/folder");
        }

        [Test]
        public void use_separators_for_underscores_if_not_a_route_input()
        {
            MethodToUrlBuilder.Alter(theRoute, "path_folder1_folder2", theProperties);

            theRoute.Pattern.ShouldEqual("path/folder1/folder2");
        }

        [Test]
        public void two_underscores_in_a_row_are_considered_to_be_an_underscore()
        {
            MethodToUrlBuilder.Alter(theRoute, "path_folder1__folder2", theProperties);
            theRoute.Pattern.ShouldEqual("path/folder1/_folder2");
        }

        [Test]
        public void three_underscores_in_a_row_are_a_dash()
        {
            MethodToUrlBuilder.Alter(theRoute, "path_folder1___folder2", theProperties);
            theRoute.Pattern.ShouldEqual("path/folder1-folder2");
        }
    }
}