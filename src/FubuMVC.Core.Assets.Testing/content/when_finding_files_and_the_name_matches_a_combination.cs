using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class when_finding_files_and_the_name_matches_a_combination : InteractionContext<ContentPlanner>
    {
        private AssetFile[] theFiles;
        private ScriptFileCombination theCombination;

        protected override void beforeEach()
        {
            theFiles = new AssetFile[]{
                new AssetFile("a.js"), 
                new AssetFile("b.js"), 
                new AssetFile("c.js"), 
                new AssetFile("d.js"), 
            };

            theCombination = new ScriptFileCombination("combo1", theFiles);
            MockFor<IAssetCombinationCache>().Stub(x => x.FindCombination(theCombination.Name))
                .Return(theCombination);


        }

        [Test]
        public void find_files_should_return_all_the_files_from_the_combination_found_in_the_cache()
        {
            ClassUnderTest.FindFiles(theCombination.Name).ShouldHaveTheSameElementsAs(theFiles);
        }
    }
}