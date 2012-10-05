using System.Linq;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetRequirementsTester : InteractionContext<AssetRequirements>
    {
        private void scriptExists(string name)
        {
            MockFor<IAssetFileGraph>().Stub(x => x.Find(name))
                .Return(new AssetFile(name));
        }

        private void scriptDoesNotExist(string name)
        {
            MockFor<IAssetFileGraph>().Stub(x => x.Find(name))
                .Return(null);
        }

        [Test]
        public void do_not_double_dip_a_requested_file()
        {
            ClassUnderTest.Require("jquery.js");
            ClassUnderTest.Require("jquery.js");
            ClassUnderTest.Require("jquery.js");

            ClassUnderTest.AllRequestedAssets.Single().ShouldEqual("jquery.js");
        }

        [Test]
        public void use_file_if_exists_negative_case()
        {
            scriptDoesNotExist("jquery.js");
            ClassUnderTest.UseAssetIfExists("jquery.js");

            ClassUnderTest.AllRequestedAssets.Any().ShouldBeFalse();
        }

        [Test]
        public void use_file_if_exists_positive_case()
        {
            scriptExists("jquery.js");
            ClassUnderTest.UseAssetIfExists("jquery.js");

            ClassUnderTest.AllRequestedAssets.Single().ShouldEqual("jquery.js");
        }
    }

    [TestFixture]
    public class when_dequeue_ing_by_mimetype : InteractionContext<AssetRequirements>
    {
        protected override void beforeEach()
        {
            // The AssetDependencyFinderCache is simple enough to use as is in UT's
            Services.Inject<IAssetDependencyFinder>(new AssetDependencyFinderCache(new AssetGraph()));
        }

        [Test]
        public void dequeue_all_assets()
        {
            ClassUnderTest.Require("main.css", "a.css", "b.css");
            ClassUnderTest.Require("main.js", "a.js", "b.js");

            ClassUnderTest.DequeueAssetsToRender().OrderBy(x => x.MimeType.Value)
                .ShouldHaveTheSameElementsAs(
                    AssetPlanKey.For(MimeType.Javascript, "a.js", "b.js", "main.js"),
                    AssetPlanKey.For(MimeType.Css, "a.css", "b.css", "main.css")
                );
        }

        [Test]
        public void dequeue_all_assets_latches()
        {
            ClassUnderTest.Require("main.css", "a.css", "b.css");
            ClassUnderTest.Require("main.js", "a.js", "b.js");

            ClassUnderTest.DequeueAssetsToRender();

            ClassUnderTest.Require("c.css", "d.css");
            ClassUnderTest.Require("c.js", "d.js");

            ClassUnderTest.DequeueAssetsToRender().OrderBy(x => x.MimeType.Value)
                .ShouldHaveTheSameElementsAs(
                    AssetPlanKey.For(MimeType.Javascript, "c.js", "d.js"),
                    AssetPlanKey.For(MimeType.Css, "c.css", "d.css")
                );
        }

        [Test]
        public void dequeue_assets_should_record_they_are_rendered()
        {
            ClassUnderTest.Require("jquery.js","jquery.foo.js","jquery.bar.js","jquery.baz.js");
            // Dequeue jquery
            ClassUnderTest.DequeueAssets(MimeType.Javascript, "jquery.js")
                .ShouldHaveTheSameElementsAs(AssetPlanKey.For(MimeType.Javascript,"jquery.js"));

            //Now dequeue all
            ClassUnderTest.Require("jquery.js", "jquery.foo.js", "jquery.bar.js", "jquery.baz.js");

            ClassUnderTest.DequeueAssetsToRender()
                .ShouldHaveTheSameElementsAs(
                    AssetPlanKey.For(MimeType.Javascript,"jquery.bar.js","jquery.baz.js","jquery.foo.js")
                );
        }
        [Test]
        public void dequeue_multiple_assets_should_record_they_are_rendered()
        {
            ClassUnderTest.Require("jquery.js", "jquery.foo.js", "jquery.bar.js", "jquery.baz.js");
            // Dequeue jquery
            ClassUnderTest.DequeueAssets(MimeType.Javascript, "jquery.js", "jquery.baz.js")
                .ShouldHaveTheSameElementsAs(AssetPlanKey.For(MimeType.Javascript, "jquery.js", "jquery.baz.js"));

            //Now dequeue all
            ClassUnderTest.Require("jquery.js", "jquery.foo.js", "jquery.bar.js", "jquery.baz.js");

            ClassUnderTest.DequeueAssetsToRender()
                .ShouldHaveTheSameElementsAs(
                    AssetPlanKey.For(MimeType.Javascript, "jquery.bar.js", "jquery.foo.js")
                );
        }



        [Test]
        public void dequeue_assets_by_mimetype()
        {
            ClassUnderTest.Require("main.css", "a.css", "b.css");
            ClassUnderTest.Require("main.js", "a.js", "b.js");

            ClassUnderTest.DequeueAssetsToRender(MimeType.Javascript)
                .ShouldHaveTheSameElementsAs("a.js", "b.js", "main.js");
        }

        [Test]
        public void dequeue_assets_by_mimetype_latches()
        {
            ClassUnderTest.Require("main.css", "a.css", "b.css");
            ClassUnderTest.Require("main.js", "a.js", "b.js");

            ClassUnderTest.DequeueAssetsToRender(MimeType.Javascript);

            ClassUnderTest.Require("d.js", "e.js", "f.css");

            ClassUnderTest.DequeueAssetsToRender(MimeType.Javascript)
                .ShouldHaveTheSameElementsAs("d.js", "e.js");
        }
    }

    [TestFixture]
    public class when_asking_script_requirements_for_scripts_to_write : InteractionContext<AssetRequirements>
    {
        protected override void beforeEach()
        {
            var assetGraph = new AssetGraph();
            assetGraph.Dependency("a.js", "b.js");
            assetGraph.Dependency("a.js", "c.js");
            assetGraph.Dependency("d.js", "e.js");
            assetGraph.Dependency("d.js", "b.js");
            assetGraph.CompileDependencies(new PackageLog());
            Services.Inject<IAssetDependencyFinder>(new AssetDependencyFinderCache(assetGraph));
        }

        [Test]
        public void should_not_write_the_same_scripts_more_than_once()
        {
            // ask for a & f, get b,c,a,f
            ClassUnderTest.Require("a.js"); // depends on b & c
            ClassUnderTest.Require("f.js"); // no dependencies


            ClassUnderTest.DequeueAssetsToRender(MimeType.Javascript).ShouldHaveTheSameElementsAs("b.js", "c.js", "f.js",
                                                                                                  "a.js");
            // ask for d, get d,e (not b, since it was already written)

            ClassUnderTest.Require("d.js"); // depends on e and b
            var assets = ClassUnderTest.DequeueAssetsToRender(MimeType.Javascript);


            assets.ShouldHaveTheSameElementsAs("e.js", "d.js");
        }
    }
}