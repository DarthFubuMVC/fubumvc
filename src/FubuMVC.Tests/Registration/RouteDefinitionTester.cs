using System.Linq;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class RouteDefinitionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            UrlContext.Reset();
        }

        #endregion

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


        [Test]
        public void create_default_value_for_a_route()
        {
            var url = new RouteDefinition<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            url.RouteInputFor("InPath").DefaultValue = "something";
            Route route = url.ToRoute();

            route.Defaults["InPath"].ShouldEqual("something");
        }

        [Test]
        public void create_two_default_values_for_a_route()
        {
            var url = new RouteDefinition<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);
            url.RouteInputFor("InPath").DefaultValue = "something";
            url.RouteInputFor("AlsoInPath").DefaultValue = "something else";
            Route route = url.ToRoute();

            route.Defaults.Count().ShouldEqual(2);
        }


        [Test]
        public void create_url_for_a_route_with_two_parameters()
        {
            var url = new RouteDefinition<SampleViewModel>("my/sample/{InPath}/{AlsoInPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);

            url.ToRoute().Url.ShouldEqual("my/sample/{InPath}/{AlsoInPath}");
        }

        [Test]
        public void create_url_will_escape_the_url()
        {
            var url = new RouteDefinition<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);

            url.CreateUrl(new SampleViewModel
            {
                InPath = "some text"
            }).ShouldEqual("/test/edit/some%20text");
        }

        [Test]
        public void create_url_with_input_model()
        {
            var url = new RouteDefinition<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);

            url.CreateUrl(new SampleViewModel
            {
                InPath = "5"
            }).ShouldEqual("/test/edit/5");
        }

        [Test]
        public void create_url_with_input_model_and_default_value_for_optional_input()
        {
            var url = new RouteDefinition<SampleViewModelWithInputs>("test/edit/{OptionalInput}");
            url.AddRouteInput(x => x.OptionalInput);

            url
                .CreateUrl(new SampleViewModelWithInputs
                {
                    OptionalInput = "a"
                })
                .ShouldEndWith("/test/edit/a");
        }

        [Test]
        public void create_url_with_input_model_and_default_value_for_required_input()
        {
            var url = new RouteDefinition<SampleViewModelWithInputs>("test/edit/{RequiredInput}");
            url.AddRouteInput(x => x.RequiredInput);

            url
                .CreateUrl(new SampleViewModelWithInputs
                {
                    RequiredInput = "a"
                })
                .ShouldEndWith("/test/edit/a");
        }

        [Test]
        public void create_url_with_input_model_and_no_default_value_specified_for_optional_input()
        {
            var url = new RouteDefinition<SampleViewModelWithInputs>("test/edit/{OptionalInput}");
            url.AddRouteInput(x => x.OptionalInput);

            url
                .CreateUrl(new SampleViewModelWithInputs())
                .ShouldEndWith("/test/edit/default");
        }

        [Test]
        public void create_url_with_input_model_and_no_default_value_specified_for_required_input()
        {
            var url = new RouteDefinition<SampleViewModelWithInputs>("test/edit/{RequiredInput}");
            url.AddRouteInput(x => x.RequiredInput);

            typeof (FubuException).ShouldBeThrownBy(() => url.CreateUrl(new SampleViewModelWithInputs()));
        }

        [Test]
        public void create_url_with_multiple_variables_in_path()
        {
            var url = new RouteDefinition<SampleViewModel>("test/edit/{InPath}/{AlsoInPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);

            url.CreateUrl(new SampleViewModel
            {
                InPath = "5",
                AlsoInPath = "some text"
            }).ShouldEqual("/test/edit/5/some%20text");
        }

        [Test]
        public void create_url_with_multiple_variables_in_querystring()
        {
            var url = new RouteDefinition<SampleViewModel>("/my/sample/path");
            url.AddQueryInput(x => x.InQueryString);
            url.AddQueryInput(x => x.AlsoInQueryString);

            url.CreateUrl(new SampleViewModel
            {
                InQueryString = "query",
                AlsoInQueryString = "alsoquery"
            })
                .ShouldEqual("/my/sample/path?InQueryString=query&AlsoInQueryString=alsoquery");
        }

        [Test]
        public void create_url_with_null_input_model_and_no_default_value_specified_for_optional_input()
        {
            var url = new RouteDefinition<SampleViewModelWithInputs>("test/edit/{OptionalInput}");
            url.AddRouteInput(x => x.OptionalInput);

            url
                .CreateUrl(null)
                .ShouldEndWith("/test/edit/default");
        }

        [Test]
        public void create_url_with_null_input_model_and_no_default_value_specified_for_required_input()
        {
            var url = new RouteDefinition<SampleViewModelWithInputs>("test/edit/{RequiredInput}");
            url.AddRouteInput(x => x.RequiredInput);

            typeof (FubuException).ShouldBeThrownBy(() => url.CreateUrl(null));
        }

        [Test]
        public void create_url_with_querystring_and_inputmodel()
        {
            var url = new RouteDefinition<SampleViewModel>("test/edit/{InPath}");
            url.AddRouteInput(x => x.InPath);
            url.AddQueryInput(x => x.InQueryString);

            url.CreateUrl(new SampleViewModel
            {
                InPath = "5",
                InQueryString = "query"
            }).ShouldEqual("/test/edit/5?InQueryString=query");
        }

        [Test]
        public void create_url_with_variables_in_querystring()
        {
            var url = new RouteDefinition<SampleViewModel>("/my/sample/path");
            url.AddQueryInput(x => x.InQueryString);

            url.CreateUrl(new SampleViewModel
            {
                InQueryString = "query"
            }).ShouldEqual("/my/sample/path?InQueryString=query");
        }

        [Test]
        public void does_not_create_default_for_a_simple_parameter()
        {
            var url = new RouteDefinition<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            Route route = url.ToRoute();

            route.Defaults.Count().ShouldEqual(0);
        }

        [Test]
        public void prepend_to_route()
        {
            var route = new RouteDefinition("my/sample");
            route.Prepend("area");

            route.Pattern.ShouldEqual("area/my/sample");
        }

        [Test]
        public void prepend_to_route_takes_care_of_stupid_trailing_leading_slashes()
        {
            var route = new RouteDefinition("my/sample");
            route.Prepend("area/");

            route.Pattern.ShouldEqual("area/my/sample");
        }

        [Test]
        public void route_input_should_substitute_method()
        {
            SingleProperty accessor = SingleProperty.Build<SampleViewModel>(x => x.InPath);
            var viewModel = new SampleViewModel
            {
                InPath = "5"
            };
            var routeInput = new RouteInput(accessor);

            routeInput.Substitute(viewModel, "test/edit/{InPath}").ShouldEqual("test/edit/5");
        }

        [Test]
        public void should_have_one_default_value_for_a_route_and_does_not_include_querystring_in_route()
        {
            var url = new RouteDefinition<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            url.AddQueryInput(x => x.InQueryString);
            url.RouteInputFor("InPath").DefaultValue = "something";
            url.QueryInputFor("InQueryString").DefaultValue = "querysomething";
            Route route = url.ToRoute();

            route.Defaults.Count().ShouldEqual(1);
        }

        [Test]
        public void should_have_one_default_value_for_route()
        {
            var url = new RouteDefinition<SampleViewModel>("my/sample");
            url.AddRouteInput(x => x.InPath);
            url.AddRouteInput(x => x.AlsoInPath);
            url.RouteInputFor("InPath").DefaultValue = "something";
            Route route = url.ToRoute();

            route.Defaults.Count().ShouldEqual(1);
        }
    }
}