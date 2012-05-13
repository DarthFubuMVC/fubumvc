using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Diagnostics;
using FubuMVC.Core.Packaging;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetGraphConfigurationActivatorIntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Directory.GetFiles(".", "*.script.config").Each(x => File.Delete(x));


            assets = new AssetGraph();
            activator = new AssetGraphConfigurationActivator(assets, new FileSystem(), new AssetLogsCache());

            log = new PackageLog();
        }

        #endregion

        private AssetGraph assets;
        private AssetGraphConfigurationActivator activator;
        private PackageLog log;

        [Test]
        public void read_a_directory()
        {
            new FileSystem().WriteStringToFile("something.script.config",
                                               @"
jquery is jquery.1.4.2.js
a includes b,c,d
");

            // This should be ignored
            new FileSystem().WriteStringToFile(FubuMvcPackageFacility.FubuContentFolder.AppendPath("wrong.script.config"),
                                               @"
a includes e,f,g
");

            new FileSystem().WriteStringToFile("else.script.config", @"
f extends d
g requires h
");

            activator.ReadScriptConfig(".", log);
            Debug.WriteLine(log.FullTraceText());

            assets.CompileDependencies(log);

            Assert.IsTrue(log.Success, log.FullTraceText());

            // got the alias
            assets.GetAssets(new[]{"jquery"}).Single().Name.ShouldEqual("jquery.1.4.2.js");

            // got the set
            assets.GetAssets(new[]{"a"}).Select(x => x.Name).ShouldHaveTheSameElementsAs("b",
                                                                                         "c", "d", "f");

            // got the extension
            assets.GetAssets(new[]{"d"}).Select(x => x.Name).ShouldHaveTheSameElementsAs("d",
                                                                                         "f");

            // got the requires
            assets.GetAssets(new[]{"g"}).Select(x => x.Name).ShouldHaveTheSameElementsAs("h",
                                                                                         "g");
        }

        [Test]
        public void the_files_were()
        {
            new FileSystem().WriteStringToFile("something.script.config",
                                               @"
jquery is jquery.1.4.2.js
a includes b,c,d
");

            new FileSystem().WriteStringToFile("else.script.config", @"
f extends d
g requires h
");

            activator.ReadScriptConfig(".", log);

            AssetGraphConfigurationActivator.ConfigurationFiles.ShouldContain("something.script.config".ToFullPath());
            AssetGraphConfigurationActivator.ConfigurationFiles.ShouldContain("else.script.config".ToFullPath());
        }

        [Test]
        public void read_a_directory_2()
        {
            new FileSystem().WriteStringToFile("something.asset.config",
                                               @"
jquery is jquery.1.4.2.js
a includes b,c,d
");

            new FileSystem().WriteStringToFile("else.asset.config", @"
f extends d
g requires h
");

            activator.ReadScriptConfig(".", log);
            Debug.WriteLine(log.FullTraceText());

            assets.CompileDependencies(log);

            Assert.IsTrue(log.Success, log.FullTraceText());

            // got the alias
            assets.GetAssets(new[] { "jquery" }).Single().Name.ShouldEqual("jquery.1.4.2.js");

            // got the set
            assets.GetAssets(new[] { "a" }).Select(x => x.Name).ShouldHaveTheSameElementsAs("b",
                                                                                         "c", "d", "f");

            // got the extension
            assets.GetAssets(new[] { "d" }).Select(x => x.Name).ShouldHaveTheSameElementsAs("d",
                                                                                         "f");

            // got the requires
            assets.GetAssets(new[] { "g" }).Select(x => x.Name).ShouldHaveTheSameElementsAs("h",
                                                                                         "g");
        }

        [Test]
        public void read_a_file()
        {
            new FileSystem().WriteStringToFile("something.script.config",
                                               @"
jquery is jquery.1.4.2.js
a includes b,c,d
f extends d
g requires h
");

            activator.ReadFile("something.script.config", log);
            assets.CompileDependencies(log);

            Assert.IsTrue(log.Success, log.FullTraceText());

            // got the alias
            assets.GetAssets(new[]{"jquery"}).Single().Name.ShouldEqual("jquery.1.4.2.js");

            // got the set
            assets.GetAssets(new[]{"a"}).Select(x => x.Name).ShouldHaveTheSameElementsAs("b",
                                                                                         "c", "d", "f");

            // got the extension
            assets.GetAssets(new[]{"d"}).Select(x => x.Name).ShouldHaveTheSameElementsAs("d",
                                                                                         "f");

            // got the requires
            assets.GetAssets(new[]{"g"}).Select(x => x.Name).ShouldHaveTheSameElementsAs("h",
                                                                                         "g");
        }
    }
}
