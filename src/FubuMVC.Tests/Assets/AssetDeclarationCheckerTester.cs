using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetDeclarationCheckerTester : InteractionContext<AssetDeclarationChecker>
    {
        string theAssetName = "folder/a.js";

        [SetUp]
        public void SetUp()
        {
            AssetDeclarationVerificationActivator.Latched = false;
        }

        [Test]
        public void does_not_log_a_problem_if_an_asset_file_can_be_found()
        {
            MockFor<IAssetPipeline>().Stub(x => x.Find(theAssetName)).Return(new AssetFile("something.js"));

            ClassUnderTest.VerifyFileDependency(theAssetName);

            MockFor<IPackageLog>().AssertWasNotCalled(x => x.MarkFailure(""), x => x.IgnoreArguments());
        }

        [Test]
        public void log_a_problem_if_an_asset_file_cannot_be_found()
        {
            MockFor<IAssetPipeline>().Stub(x => x.Find(theAssetName)).Return(null);

            ClassUnderTest.VerifyFileDependency(theAssetName);

            var expectedMessage = AssetDeclarationChecker.SpecifiedAssetFileDependencyDoesNotExist
                .ToFormat(theAssetName);


            MockFor<IPackageLog>().AssertWasCalled(x => x.MarkFailure(expectedMessage));
        }
    }
}