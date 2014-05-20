using System.Diagnostics;
using Bottles;
using Castle.Core.Resource;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.StructureMap;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin.Middleware
{
    [TestFixture]
    public class HtmlHeadInjectionMiddlewareTester
    {
        [SetUp]
        public void SetUp()
        {
            PackageRegistry.Properties[HtmlHeadInjectionMiddleware.TEXT_PROPERTY] = null;
        }

        [Test]
        public void can_inject_the_right_html_on_GET_for_html_text()
        {
            

            var registry = new FubuRegistry();
            registry.AlterSettings<OwinSettings>(x => {
                x.AddMiddleware<HtmlHeadInjectionMiddleware>().Arguments.With(new InjectionOptions
                {
                    Content = e => new HtmlTag("script").Attr("foo", "bar").ToString()
                });
            });

            using (var server = FubuApplication.For(registry).StructureMap().RunInMemory())
            {
                server.Scenario(_ => {
                    _.Get.Action<SimpleHtmlEndpoint>(x => x.get_html_content());

                    _.ContentShouldContain("<script foo=\"bar\"></script></head>");

                });

                server.Scenario(_ => {
                    _.Get.Action<SimpleHtmlEndpoint>(x => x.get_text_content());

                    _.ContentShouldNotContain("<script foo=\"bar\"></script></head>");
                });
            }


        }
    }

    public class SimpleHtmlEndpoint
    {
        public HtmlDocument get_html_content()
        {
            var document = new HtmlDocument();
            document.Title = "Some Html";

            return document;
        }

        public string get_text_content()
        {
            return "some text";
        }
    }
}