using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.JavascriptRouting;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.UI;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.StructureMap;
using HtmlTags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.Assets
{
    [TestFixture]
    public class JavascriptRouting
    {
        [Test]
        public void can_write_the_javascript_routes_for_all_methods()
        {
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Scenario(x =>
                {
                    x.Get.Action<JavascriptRoutesEndpoint>(e => e.get_js_router());


                    x.ContentShouldContain("myRoutes = {");


                    x.ContentShouldContain("\"m1\":{\"name\":\"m1\",\"method\":\"GET\",\"url\":\"/js/method1/{Name}\"");


                    x.ContentShouldContain("\"m2\":{\"name\":\"m2\",\"method\":\"GET\",\"url\":\"/js/method2\"}");
                    x.ContentShouldContain("\"m3\":{\"name\":\"m3\",\"method\":\"POST\",\"url\":\"/js/method3\"}");
                    x.ContentShouldContain("\"m4\":{\"name\":\"m4\",\"method\":\"PUT\",\"url\":\"/js/method3\"}");
                    x.ContentShouldContain("\"m5\":{\"name\":\"m5\",\"method\":\"DELETE\",\"url\":\"/js/method3\"}");

                    x.StatusCodeShouldBe(HttpStatusCode.OK);

                });
            }


        }

        [Test]
        public void can_swap_out_the_javascript_route_data()
        {
            var container = new Container(_ => {
                _.For<IJavascriptRouteData>().Use<FakeJavascriptRouteData>();
            });

            using (var host = FubuApplication.DefaultPolicies().StructureMap(container).RunInMemory())
            {
                host.Scenario(x =>
                {
                    x.Get.Action<JavascriptRoutesEndpoint>(e => e.get_js_router());


                    x.ContentShouldContain("myRoutes = {");


                    x.ContentShouldContain("\"m1\":{\"name\":\"m1\",\"method\":\"GET\",\"url\":\"/fake/js/method1/{Name}\"");


                    x.ContentShouldContain("\"m2\":{\"name\":\"m2\",\"method\":\"GET\",\"url\":\"/fake/js/method2\"}");
                    x.ContentShouldContain("\"m3\":{\"name\":\"m3\",\"method\":\"POST\",\"url\":\"/fake/js/method3\"}");


                    x.StatusCodeShouldBe(HttpStatusCode.OK);

                });
            }
        }

        public class FakeJavascriptRouteData : IJavascriptRouteData
        {
            public string ToUrl(RoutedChain chain)
            {
                return ("/fake/" + chain.GetRoutePattern()).Replace("//", "/");
            }

            public IEnumerable<RouteParameter> ToParameters(RoutedChain chain)
            {
                return new RouteParameter[0];
            }
        }
    }

    public class MyJsRouter : JavascriptRouter
    {
        public MyJsRouter()
        {
            Get("m1").InputType<Method1Input>();
            Get("m2").Action<JavascriptRoutesEndpoint>(x => x.get_js_method2());
            Post("m3").InputType<Method1Input>();
            Put("m4").InputType<Method1Input>();
            Delete("m5").InputType<Method1Input>();
        }
    }

    public class JavascriptRoutesEndpoint
    {
        private readonly FubuHtmlDocument _document;

        public JavascriptRoutesEndpoint(FubuHtmlDocument document)
        {
            _document = document;
        }

        public HtmlDocument get_js_router()
        {
            _document.Add(x => x.JavascriptRoutes<MyJsRouter>("myRoutes"));

            return _document;
        }

        public string get_js_method1_Name(Method1Input input)
        {
            return "something";
        }

        public string get_js_method2()
        {
            return "2";
        }

        public AjaxContinuation post_js_method3(Method1Input input)
        {
            return AjaxContinuation.Successful();
        }

        public AjaxContinuation put_js_method3(Method1Input input)
        {
            return AjaxContinuation.Successful();
        }

        public AjaxContinuation delete_js_method3(Method1Input input)
        {
            return AjaxContinuation.Successful();
        }
    }

    public class Method1Input
    {
        public string Name { get; set; }
    }
}