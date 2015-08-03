using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using HtmlTags;
using NUnit.Framework;
using Shouldly;

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
            var settings = new OwinSettings();
            settings.Middleware.OfType<MiddlewareNode<HtmlHeadInjectionMiddleware>>()
                .Any().ShouldBeFalse();
        }

        [Test]
        public void create_with_html_head_injection()
        {
            var html = new HtmlTag("script").Attr("foo", "bar").ToString();


            using (var runtime = FubuRuntime.Basic(_ =>
            {
                _.Mode = "development";
                _.AlterSettings<OwinSettings>(x =>
                {
                    x.AddMiddleware<HtmlHeadInjectionMiddleware>().Arguments.With(new InjectionOptions
                    {
                        Content = c => html
                    });
                });
            }))
            {
                var settings = runtime.Get<OwinSettings>();
                settings.Middleware.OfType<MiddlewareNode<HtmlHeadInjectionMiddleware>>()
                    .Any().ShouldBeTrue();
            }
        }
    }
}