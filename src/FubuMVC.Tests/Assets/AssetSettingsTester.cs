using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetSettingsTester
    {
        private IStaticFileRule theRule;

        [SetUp]
        public void SetUp()
        {
            theRule = new AssetSettings().As<IStaticFileRule>();
        }

        [Test]
        public void can_write_javascript_files()
        {
            theRule.IsAllowed(new FubuFile("foo.js", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("foo.coffee", null)).ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void can_write_css()
        {
            theRule.IsAllowed(new FubuFile("bar.css", null)).ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void can_write_htm_or_html()
        {
            theRule.IsAllowed(new FubuFile("bar.htm", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("bar.html", null)).ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void can_write_images()
        {
            theRule.IsAllowed(new FubuFile("bar.jpg", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("bar.gif", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("bar.tif", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("bar.png", null)).ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void none_if_the_mime_type_is_not_recognized()
        {
            theRule.IsAllowed(new FubuFile("bar.nonexistent", null)).ShouldEqual(AuthorizationRight.None);
        }

        [Test]
        public void none_if_not_an_asset_file_or_html()
        {
            theRule.IsAllowed(new FubuFile("bar.txt", null)).ShouldEqual(AuthorizationRight.None);
        }

        [Test]
        public void default_static_file_rules()
        {
            new AssetSettings().StaticFileRules
                .Select(x => x.GetType()).OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(DenyConfigRule));
        }

        private AuthorizationRight forFile(string filename)
        {
            var file = new FubuFile(filename, null);
            var owinSettings = new AssetSettings();
            owinSettings.StaticFileRules.Add(new AssetSettings());

            return owinSettings.DetermineStaticFileRights(file);
        }

        [Test]
        public void assert_static_rights()
        {
            forFile("foo.txt").ShouldEqual(AuthorizationRight.None);
            forFile("foo.config").ShouldEqual(AuthorizationRight.Deny);
            forFile("foo.jpg").ShouldEqual(AuthorizationRight.Allow);
            forFile("foo.css").ShouldEqual(AuthorizationRight.Allow);
            forFile("foo.bmp").ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void headers_in_production_mode()
        {
            FubuMode.Reset();

            var settings = new AssetSettings();
            settings.Headers.GetAllKeys().ShouldHaveTheSameElementsAs(HttpRequestHeaders.CacheControl, HttpRequestHeaders.Expires);

            settings.Headers[HttpRequestHeaders.CacheControl]().ShouldEqual("private, max-age=86400");
            settings.Headers[HttpRequestHeaders.Expires]().ShouldNotBeNull();
        }

        [Test]
        public void no_headers_in_development_mode()
        {
            FubuMode.SetUpForDevelopmentMode();

            new AssetSettings().Headers.GetAllKeys().Any().ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_creating_the_default_search
    {
        private FileSet search = new AssetSettings().CreateAssetSearch();

        [Test]
        public void exclude_should_be_null()
        {
            search.Exclude.ShouldBeNull();
        }

        [Test]
        public void should_be_deep()
        {
            search.DeepSearch.ShouldBeTrue();
        }

        [Test]
        public void should_look_for_js_files()
        {
            search.Include.ShouldContain("*.js");
        }

        [Test]
        public void should_look_for_css_files()
        {
            search.Include.ShouldContain("*.css");
        }

        [Test]
        public void should_include_all_file_types_registered_as_images()
        {
            search.Include.ShouldContain("*.bmp");
            search.Include.ShouldContain("*.png");
            search.Include.ShouldContain("*.gif");
            search.Include.ShouldContain("*.jpg");
            search.Include.ShouldContain("*.jpeg");
        }


    }
}