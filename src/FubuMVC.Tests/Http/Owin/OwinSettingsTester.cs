using System.Linq;
using System.Web;
using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using Shouldly;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class OwinSettingsTester
    {
        [Test]
        public void static_file_middle_ware_is_added_by_default()
        {
            var settings = new OwinSettings();

            settings.Middleware.OfType<MiddlewareNode<StaticFileMiddleware>>()
                .Count().ShouldBe(1);
        }

        [Test]
        public void create_with_no_html_head_injection()
        {
            FubuRuntime.Properties.ClearAll();

            var settings = new OwinSettings();
            settings.Middleware.OfType<MiddlewareNode<HtmlHeadInjectionMiddleware>>()
                .Any().ShouldBeFalse();
        }

        [Test]
        public void create_with_html_head_injection()
        {
            FubuRuntime.Properties[HtmlHeadInjectionMiddleware.TEXT_PROPERTY] =
                new HtmlTag("script").Attr("foo", "bar").ToString();

            using (var runtime = FubuRuntime.Basic(_ => _.Mode = "development"))
            {
                var settings = runtime.Factory.Get<OwinSettings>();
                settings.Middleware.OfType<MiddlewareNode<HtmlHeadInjectionMiddleware>>()
                    .Any().ShouldBeTrue();
            }




        }
    }
}