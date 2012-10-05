using System.Linq;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class when_finding_files_and_name_only_matches_a_file : InteractionContext<ContentPlanner>
    {
        private AssetFile theFile;

        protected override void beforeEach()
        {
            theFile = new AssetFile("script.js");

            MockFor<IAssetCombinationCache>().Stub(x => x.FindCombination(theFile.Name)).Return(null);

            MockFor<IAssetFileGraph>().Stub(x => x.Find(theFile.Name)).Return(theFile);
        }

        [Test]
        public void find_files_should_only_return_the_one_file_found_from_IAssetPipeline()
        {
            ClassUnderTest.FindFiles(theFile.Name).Single().ShouldBeTheSameAs(theFile);
        }
    }
}