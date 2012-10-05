using System;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Diagnostics;
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
        private string _provenance = "test_provenance";
		
		protected override void beforeEach ()
		{
            AssetDeclarationVerificationActivator.Latched = false;
 
            var logs = new AssetLogsCache();

            logs.FindByName(theAssetName)
                .Add(_provenance, "Test log message 1");
            logs.FindByName(theAssetName)
                .Add(_provenance, "Test log message 2");

            Services.Inject(logs);
        }

        [Test]
        public void does_not_log_a_problem_if_an_asset_file_can_be_found()
        {
            MockFor<IAssetFileGraph>().Stub(x => x.Find(theAssetName)).Return(new AssetFile("something.js"));

            ClassUnderTest.VerifyFileDependency(theAssetName);

            MockFor<IPackageLog>().AssertWasNotCalled(x => x.MarkFailure(""), x => x.IgnoreArguments());
        }

        [Test]
        public void asset_the_log_message_contains_provenance()
        {
             MockFor<IAssetFileGraph>().Stub(x => x.Find(theAssetName)).Return(null);

            ClassUnderTest.VerifyFileDependency(theAssetName);

            var expectedMessage = AssetDeclarationChecker.GetErrorMessage(theAssetName, MockFor<AssetLogsCache>());

            Console.Write(expectedMessage);

            expectedMessage.ShouldContainAllOf(_provenance, "message 1", "message 2");

        }

        [Test]
        public void log_a_problem_if_an_asset_file_cannot_be_found()
        {
            MockFor<IAssetFileGraph>().Stub(x => x.Find(theAssetName)).Return(null);

            ClassUnderTest.VerifyFileDependency(theAssetName);

            var expectedMessage = AssetDeclarationChecker.GetErrorMessage(theAssetName, MockFor<AssetLogsCache>());


            MockFor<IPackageLog>().AssertWasCalled(x => x.MarkFailure(expectedMessage));
        }
    }
}