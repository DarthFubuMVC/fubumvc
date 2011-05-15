using System.Collections.Generic;
using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class MethodToUrlBuilderIntegratedTester
    {
        private BehaviorChain theChain;

        [SetUp]
        public void SetUp()
        {
            var graph = new FubuRegistry(x =>
            {
                x.Actions.IncludeType<MethodAction>();

            }).BuildGraph();

            theChain = graph.BehaviorFor<MethodAction>(x => x.Get_cases_from_Start_to_End(null));
        }

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
        public int Start { get; set;}
        public int End { get; set;}
    }


    [TestFixture]
    public class MethodToUrlBuilderTester
    {
        private List<string> theProperties;
        private RouteDefinition theRoute;

        [SetUp]
        public void SetUp()
        {
            theProperties = new List<string>();
            theRoute = new RouteDefinition("");
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
        public void create_http_constraint_for_get()
        {
            MethodToUrlBuilder.Alter(theRoute, "Get_path", theProperties, x => Debug.WriteLine(x));

            theRoute.Pattern.ShouldEqual("path");
            theRoute.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Test]
        public void allow_get_as_a_prefix()
        {
            MethodToUrlBuilder.Alter(theRoute, "GetPath", theProperties, x => Debug.WriteLine(x));

            theRoute.Pattern.ShouldEqual("getpath");
            theRoute.AllowedHttpMethods.Any().ShouldBeFalse();
        }

        [Test]
        public void create_http_constraint_for_post()
        {
            MethodToUrlBuilder.Alter(theRoute, "post_path", theProperties, x => Debug.WriteLine(x));

            theRoute.Pattern.ShouldEqual("path");
            theRoute.AllowedHttpMethods.ShouldHaveTheSameElementsAs("POST");
        }

        [Test]
        public void create_http_constraint_for_put()
        {
            MethodToUrlBuilder.Alter(theRoute, "Put_path", theProperties, x => Debug.WriteLine(x));

            theRoute.Pattern.ShouldEqual("path");
            theRoute.AllowedHttpMethods.ShouldHaveTheSameElementsAs("PUT");
        }

        [Test]
        public void use_separators_for_underscores_if_not_a_route_input()
        {
            MethodToUrlBuilder.Alter(theRoute, "path_folder1_folder2", theProperties, x => Debug.WriteLine(x));

            theRoute.Pattern.ShouldEqual("path/folder1/folder2");
        }

        [Test]
        public void use_separator_and_substitution_for_matching_property()
        {
            theProperties.Add("Input1");
            MethodToUrlBuilder.Alter(theRoute, "path_Input1_folder", theProperties, x => Debug.WriteLine(x));

            theRoute.Pattern.ShouldEqual("path/{Input1}/folder");
        }

        [Test]
        public void multiple_substitutions()
        {
            theProperties.Add("Start");
            theProperties.Add("End");

            MethodToUrlBuilder.Alter(theRoute, "cases_from_Start_to_End", theProperties, x => Debug.WriteLine(x));

            theRoute.Pattern.ShouldEqual("cases/from/{Start}/to/{End}");
        }

        [Test]
        public void multiple_substitutions_and_folders_and_constraint()
        {
            theProperties.Add("Start");
            theProperties.Add("End");

            MethodToUrlBuilder.Alter(theRoute, "get_cases_from_Start_to_End", theProperties, x => Debug.WriteLine(x));

            theRoute.Pattern.ShouldEqual("cases/from/{Start}/to/{End}");
            theRoute.AllowedHttpMethods.ShouldContain("GET");
        }

    }

}