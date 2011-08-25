using System;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class ContentPlannerTester : InteractionContext<ContentPlanner>
    {
        [Test]
        public void throw_argument_out_of_range_exception_when_findfiles_cannot_find_anything()
        {
            MockFor<IAssetCombinationCache>().Stub(x => x.FindCombination("script1")).Return(null);
            MockFor<IAssetPipeline>().Stub(x => x.Find("script1")).Return(null);

            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.FindFiles("script1");
            });
        }
    }

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

    [TestFixture]
    public class when_finding_files_and_name_only_matches_a_file : InteractionContext<ContentPlanner>
    {
        private AssetFile theFile;

        protected override void beforeEach()
        {
            theFile = new AssetFile("script.js");

            MockFor<IAssetCombinationCache>().Stub(x => x.FindCombination(theFile.Name)).Return(null);

            MockFor<IAssetPipeline>().Stub(x => x.Find(theFile.Name)).Return(theFile);
        }

        [Test]
        public void find_files_should_only_return_the_one_file_found_from_IAssetPipeline()
        {
            ClassUnderTest.FindFiles(theFile.Name).Single().ShouldBeTheSameAs(theFile);
        }
    }
}