using System.Collections.Generic;
using System.Diagnostics;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Routes;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Registration.Conventions
{
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