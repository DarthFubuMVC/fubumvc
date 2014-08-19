using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.StructureMap;
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
        public void public_folder_is_public()
        {
            new AssetSettings().PublicFolder.ShouldEqual("public");
        }

        [Test]
        public void mode_is_anywhere_by_default()
        {
            new AssetSettings().Mode.ShouldEqual(SearchMode.Anywhere);
        }

        [Test]
        public void version_should_be_null_by_default()
        {
            new AssetSettings().Version.ShouldBeNull();
        }

        [Test]
        public void exclusions_includes_node_modules()
        {
            var settings = new AssetSettings();
            settings.Exclusions.ShouldStartWith("node_modules/*");
        }

        [Test]
        public void add_exclusions()
        {
            var settings = new AssetSettings();
            settings.Exclude("something.js");
            settings.Exclude("node-content");

            settings.CreateAssetSearch().Exclude.ShouldEqual("node_modules/*;something.js;node-content");
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
                .ShouldHaveTheSameElementsAs(typeof (DenyConfigRule));
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
            settings.Headers.GetAllKeys()
                .ShouldHaveTheSameElementsAs(HttpRequestHeaders.CacheControl, HttpRequestHeaders.Expires);

            settings.Headers[HttpRequestHeaders.CacheControl]().ShouldEqual("private, max-age=86400");
            settings.Headers[HttpRequestHeaders.Expires]().ShouldNotBeNull();
        }

        [Test]
        public void no_headers_in_development_mode()
        {
            FubuMode.SetUpForDevelopmentMode();

            new AssetSettings().Headers.GetAllKeys().Any().ShouldBeFalse();
        }

        [Test]
        public void determine_the_public_folder_with_no_version()
        {
            new FileSystem().CreateDirectory(
                FubuMvcPackageFacility.GetApplicationPath().AppendPath("public").ToFullPath());

            var settings = new AssetSettings
            {
                Version = null
            };

            settings.DeterminePublicFolder()
                .ShouldEqual(FubuMvcPackageFacility.GetApplicationPath().AppendPath("public"));
        }

        [Test]
        public void determine_the_public_folder_with_a_non_null_but_nonexistent_version()
        {
            new FileSystem().CreateDirectory(
                FubuMvcPackageFacility.GetApplicationPath().AppendPath("public").ToFullPath());

            var settings = new AssetSettings
            {
                Version = Guid.NewGuid().ToString()
            };

            settings.DeterminePublicFolder()
                .ShouldEqual(FubuMvcPackageFacility.GetApplicationPath().AppendPath("public").ToFullPath());
        }

        [Test]
        public void determine_the_public_folder_when_the_version_does_exist()
        {
            new FileSystem().CreateDirectory(
                FubuMvcPackageFacility.GetApplicationPath().AppendPath("public").ToFullPath());
            new FileSystem().CreateDirectory(
                FubuMvcPackageFacility.GetApplicationPath().AppendPath("public", "1.0.1").ToFullPath());

            var settings = new AssetSettings
            {
                Version = "1.0.1"
            };

            settings.DeterminePublicFolder()
                .ShouldEqual(FubuMvcPackageFacility.GetApplicationPath().AppendPath("public", "1.0.1").ToFullPath());
        }

        [Test]
        public void default_allowable_mimetypes_includes_fonts()
        {
            var settings = new AssetSettings();
            var search = settings.CreateAssetSearch();

            search.Include.ShouldContain(".svg");
            search.Include.ShouldContain(".eot");
            search.Include.ShouldContain(".ttf");
            search.Include.ShouldContain(".woff");
        }

        [Test]
        public void template_destination_by_default_should_be_underscore_templates()
        {
            new AssetSettings().TemplateDestination.ShouldEqual("_templates");
        }

        [Test]
        public void the_default_cultures_for_templates_is_only_en_US_because_MURICA()
        {
            new AssetSettings().TemplateCultures.ShouldHaveTheSameElementsAs("en-US");
        }

        [Test]
        public void default_content_files()
        {
            var settings = new AssetSettings();

            settings.ContentMatches.ShouldContain(".htm");
            settings.ContentMatches.ShouldContain(".html");
            settings.ContentMatches.ShouldContain(".txt");
        }
    }

    [TestFixture]
    public class when_creating_the_default_search
    {
        private FileSet search = new AssetSettings().CreateAssetSearch();

        [Test]
        public void exclude_should_include_node_modules()
        {
            search.Exclude.ShouldEqual("node_modules/*");
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

        [Test]
        public void find_files_for_public_folder_only()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<AssetSettings>(x => { x.Mode = SearchMode.PublicFolderOnly; });

            using (var runtime = FubuApplication.For(registry).StructureMap().Bootstrap())
            {
                var graph = runtime.Factory.Get<IAssetFinder>().FindAll();
                graph.Assets.OrderBy(x => x.Url).Select(x => x.Url)
                    .ShouldHaveTheSameElementsAs("public/1.0.1/d.js", "public/1.0.1/e.js", "public/1.0.1/f.js",
                        "public/javascript/a.js", "public/javascript/b.js", "public/javascript/c.js");
            }
        }

        [Test]
        public void find_files_for_public_folder_with_version()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<AssetSettings>(x => {
                x.Mode = SearchMode.PublicFolderOnly;
                x.Version = "1.0.1";
            });

            using (var runtime = FubuApplication.For(registry).StructureMap().Bootstrap())
            {
                var graph = runtime.Factory.Get<IAssetFinder>().FindAll();
                graph.Assets.OrderBy(x => x.Url).Select(x => x.Url)
                    .ShouldHaveTheSameElementsAs("public/1.0.1/d.js", "public/1.0.1/e.js", "public/1.0.1/f.js");
            }
        }
    }

    [TestFixture]
    public class when_creating_a_file_watcher_manifest
    {
        [Test]
        public void set_the_public_folder_if_in_that_mode()
        {
            var settings = new AssetSettings
            {
                Mode = SearchMode.PublicFolderOnly,
                PublicFolder = "public"
            };

            var manifest = settings.CreateFileWatcherManifest();

            manifest.PublicAssetFolder.ShouldEqual(
                FubuMvcPackageFacility.GetApplicationPath().AppendPath("public").Replace('\\', '/'));

        }

        [Test]
        public void no_public_folder_if_in_anywhere_mode()
        {
            var settings = new AssetSettings
            {
                Mode = SearchMode.Anywhere
            };

            settings.CreateFileWatcherManifest().PublicAssetFolder.ShouldBeEmpty();
        }

        [Test]
        public void adds_content_extensions()
        {
            var settings = new AssetSettings();
            settings.ContentMatches.Add(".foo");

            var manifest = settings.CreateFileWatcherManifest();

            manifest.ContentMatches.ShouldContain(".foo");
            manifest.ContentMatches.ShouldContain(".htm");
            manifest.ContentMatches.ShouldContain(".html");
        }

        [Test]
        public void adds_all_the_default_asset_extensions()
        {
            var settings = new AssetSettings();
            var manifest = settings.CreateFileWatcherManifest();

            manifest.AssetExtensions.ShouldContain(".js");
            manifest.AssetExtensions.ShouldContain(".css");
            manifest.AssetExtensions.ShouldContain(".jpeg");
            manifest.AssetExtensions.ShouldContain(".jpg");
            manifest.AssetExtensions.ShouldContain(".bmp");
        }

        [Test]
        public void adds_the_user_supplied_extensions()
        {
            var settings = new AssetSettings();
            var manifest = settings.CreateFileWatcherManifest();

            manifest.AssetExtensions.ShouldContain(".svg");
            manifest.AssetExtensions.ShouldContain(".ttf");
            manifest.AssetExtensions.ShouldContain(".eot");
        }
    }
}