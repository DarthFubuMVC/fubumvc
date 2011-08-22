using System.Linq;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Content;
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
            MockFor<IContentFolderService>().Stub(x => x.FileExists(ContentType.scripts, name))
                .Return(true);
        }

        private void scriptDoesNotExist(string name)
        {
            MockFor<IContentFolderService>().Stub(x => x.FileExists(ContentType.scripts, name))
                .Return(false);
        }

        [Test]
        public void do_not_double_dip_a_requested_file()
        {
            ClassUnderTest.Require("jquery.js");
            ClassUnderTest.Require("jquery.js");
            ClassUnderTest.Require("jquery.js");

            ClassUnderTest.GetAssetsToRenderOLD().Single().ShouldEqual("jquery.js");
        }

        [Test]
        public void use_file_if_exists_negative_case()
        {
            scriptDoesNotExist("jquery.js");
            ClassUnderTest.UseFileIfExists("jquery.js");

            ClassUnderTest.GetAssetsToRenderOLD().Any().ShouldBeFalse();
        }

        [Test]
        public void use_file_if_exists_positive_case()
        {
            scriptExists("jquery.js");
            ClassUnderTest.UseFileIfExists("jquery.js");

            ClassUnderTest.GetAssetsToRenderOLD().Single().ShouldEqual("jquery.js");
        }
    }

    [TestFixture]
    public class when_asking_script_requirements_for_scripts_to_write : InteractionContext<AssetRequirements>
    {
        protected override void beforeEach()
        {
            var assetGraph = new AssetGraph();
            assetGraph.Dependency("a", "b");
            assetGraph.Dependency("a", "c");
            assetGraph.Dependency("d", "e");
            assetGraph.Dependency("d", "b");
            assetGraph.CompileDependencies(new PackageLog());
            Services.Inject(assetGraph);
        }

        [Test]
        public void should_not_write_the_same_scripts_more_than_once()
        {
            // ask for a & f, get b,c,a,f
            ClassUnderTest.Require("a"); // depends on b & c
            ClassUnderTest.Require("f"); // no dependencies

            ClassUnderTest.GetAssetsToRenderOLD().ShouldHaveTheSameElementsAs("b", "c", "f", "a");
            // ask for d, get d,e (not b, since it was already written)

            ClassUnderTest.Require("d"); // depends on e and b
            ClassUnderTest.GetAssetsToRenderOLD().ShouldHaveTheSameElementsAs("e", "d");
        }
    }
}