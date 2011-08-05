using System.Diagnostics;
using System.IO;
using System.Linq;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Content;
using FubuMVC.Core.Packaging;
using FubuTestingSupport;
using NUnit.Framework;
using System.Collections.Generic;

namespace FubuMVC.Tests.Content
{
    [TestFixture]
    public class ScriptGraphConfigurationActivatorIntegrationTester
    {
        private AssetGraph assets;
        private ScriptGraphConfigurationActivator activator;
        private PackageLog log;

        [SetUp]
        public void SetUp()
        {
            Directory.GetFiles(".", "*.script.config").Each(x => File.Delete(x));


            assets = new AssetGraph();
            activator = new ScriptGraphConfigurationActivator(assets, new FileSystem());

            log = new PackageLog();
        }

        [Test]
        public void read_a_file()
        {
            new FileSystem().WriteStringToFile("something.script.config", @"
jquery is jquery.1.4.2.js
a includes b,c,d
f extends d
g requires h
");

            activator.ReadFile("something.script.config", log);
            assets.CompileDependencies(log);

            Assert.IsTrue(log.Success, log.FullTraceText());

            // got the alias
            assets.GetScripts(new string[]{"jquery"}).Single().Name.ShouldEqual("jquery.1.4.2.js");
        
            // got the set
            assets.GetScripts(new string[]{"a"}).Select(x => x.Name).ShouldHaveTheSameElementsAs("b", "c", "d", "f");

            // got the extension
            assets.GetScripts(new string[]{"d"}).Select(x => x.Name).ShouldHaveTheSameElementsAs("d", "f");

            // got the requires
            assets.GetScripts(new string[]{"g"}).Select(x => x.Name).ShouldHaveTheSameElementsAs("h", "g");
        }

        [Test]
        public void read_a_directory()
        {
            new FileSystem().WriteStringToFile("something.script.config", @"
jquery is jquery.1.4.2.js
a includes b,c,d
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
            assets.GetScripts(new string[] { "jquery" }).Single().Name.ShouldEqual("jquery.1.4.2.js");

            // got the set
            assets.GetScripts(new string[] { "a" }).Select(x => x.Name).ShouldHaveTheSameElementsAs("b", "c", "d", "f");

            // got the extension
            assets.GetScripts(new string[] { "d" }).Select(x => x.Name).ShouldHaveTheSameElementsAs("d", "f");

            // got the requires
            assets.GetScripts(new string[] { "g" }).Select(x => x.Name).ShouldHaveTheSameElementsAs("h", "g");
        }
    }
}