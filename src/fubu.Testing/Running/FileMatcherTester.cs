using System;
using Fubu.Running;
using FubuCore;
using FubuMVC.Core;
using NUnit.Framework;
using FubuTestingSupport;

namespace fubu.Testing.Running
{
    [TestFixture]
    public class FileMatcherTester
    {
        private FileMatcher theMatcher;

        [SetUp]
        public void SetUp()
        {
            var manifest = new FileWatcherManifest();
            manifest.ContentMatches = new string[]{"*.cshtml", "bindings.xml"};

            theMatcher = new FileMatcher(manifest);

            theMatcher.Add(new EndsWithPatternMatch(FileChangeCategory.Application, "*.asset.config"));
            theMatcher.Add(new ExtensionMatch(FileChangeCategory.Content, "*.css"));
            theMatcher.Add(new ExtensionMatch(FileChangeCategory.Content, "*.spark"));

        }

        [Test]
        public void changing_the_configuration_file_forces_an_appdomain_change()
        {
            theMatcher.CategoryFor(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)
                .ShouldEqual(FileChangeCategory.AppDomain);
        }

        [Test]
        public void uses_full_file_content_match_from_manifest()
        {
            theMatcher.CategoryFor("bindings.xml").ShouldEqual(FileChangeCategory.Content);
            theMatcher.CategoryFor("bindings2.xml").ShouldEqual(FileChangeCategory.Nothing);
        }

        [Test]
        public void uses_extension_matching_on_content_from_the_manifest()
        {
            theMatcher.CategoryFor("foo.cshtml").ShouldEqual(FileChangeCategory.Content);
        }


        [Test]
        public void always_return_app_domain_for_files_in_appdomain()
        {
            theMatcher.CategoryFor("bin\\foo").ShouldEqual(FileChangeCategory.AppDomain);
            theMatcher.CategoryFor("bin\\innocuous.txt").ShouldEqual(FileChangeCategory.AppDomain);
        }

        [Test]
        public void can_return_application()
        {
            theMatcher.CategoryFor("diagnostics.asset.config").ShouldEqual(FileChangeCategory.Application);
        }

        [Test]
        public void web_config_is_app_domain()
        {
            theMatcher.CategoryFor("web.config").ShouldEqual(FileChangeCategory.AppDomain);
        }

        [Test]
        public void dll_is_app_domain()
        {
            theMatcher.CategoryFor("something.dll").ShouldEqual(FileChangeCategory.AppDomain);
        }

        [Test]
        public void exe_is_app_domain()
        {
            theMatcher.CategoryFor("something.exe").ShouldEqual(FileChangeCategory.AppDomain);
        }

        [Test]
        public void match_on_extension()
        {
            theMatcher.CategoryFor("something.spark").ShouldEqual(FileChangeCategory.Content);
            theMatcher.CategoryFor("something.css").ShouldEqual(FileChangeCategory.Content);
        }

        [Test]
        public void match_on_nothing_is_nothing()
        {
            theMatcher.CategoryFor("foo.txt").ShouldEqual(FileChangeCategory.Nothing);
        }


        [Test]
        public void match_on_assets_from_anywhere()
        {
            var manifest = new FileWatcherManifest {AssetExtensions = new string[] {"*.css", "*.js", "*.weird"}};

            var matcher = new FileMatcher(manifest);

            matcher.CategoryFor("foo/bar.js").ShouldEqual(FileChangeCategory.Content);
            matcher.CategoryFor("foo/bar.css").ShouldEqual(FileChangeCategory.Content);
            matcher.CategoryFor("bar.css").ShouldEqual(FileChangeCategory.Content);
            matcher.CategoryFor("foo.js").ShouldEqual(FileChangeCategory.Content);
            matcher.CategoryFor("foo.weird").ShouldEqual(FileChangeCategory.Content);
            matcher.CategoryFor("weird.foo").ShouldEqual(FileChangeCategory.Nothing);
        }

        [Test]
        public void match_on_assets_from_public_folder()
        {
            var manifest = new FileWatcherManifest
            {
                AssetExtensions = new string[] { "*.css", "*.js", "*.weird" },
                PublicAssetFolder = "public/v1"
            };

            var matcher = new FileMatcher(manifest);

            matcher.CategoryFor("public/v1/foo/bar.js").ShouldEqual(FileChangeCategory.Content);
            matcher.CategoryFor("public/v1/foo/bar.css").ShouldEqual(FileChangeCategory.Content);
            matcher.CategoryFor("bar.css").ShouldEqual(FileChangeCategory.Nothing);
            matcher.CategoryFor("foo.js").ShouldEqual(FileChangeCategory.Nothing);
        }
    }


}