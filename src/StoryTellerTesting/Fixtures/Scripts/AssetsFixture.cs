using System.Collections.Generic;
using FubuTestApplication;
using Serenity;
using StoryTeller;
using StoryTeller.Engine;
using System.Linq;

namespace IntegrationTesting.Fixtures.Scripts
{
    public class AssetsFixture : Fixture
    {
        private readonly IList<string> _requirements = new List<string>();
        private Serenity.ApplicationDriver _driver;

        public IGrammar IfTheAssetsAre()
        {
            return Embed<AssetPipelineSetupFixture>("If the asset pipeline is configured as");
        }

        [Hidden]
        public void RequireFile(string File)
        {
            _requirements.Fill(File);
        }

        public override void SetUp(ITestContext context)
        {
            _driver = context.Retrieve<Serenity.ApplicationDriver>();
        }

        public IGrammar RequestPageWithAssets()
        {
            return this["RequireFile"]
                .AsTable("Request a page with these required assets")
                .After(requestPage);
        }

        private AssetTagsState _assetTagState;

        private void requestPage()
        {
            var request = new ScriptRequest{
                Mandatories = _requirements.Join(",")
            };

            _driver.NavigateTo(request);

            _assetTagState = _driver.GetCurrentScreen().GetAssetDeclarations();
        }

        public IGrammar TheScriptTagsShouldBe()
        {
            return VerifyStringList(() => _assetTagState.Scripts.Select(x => x.AssetName()))
                .Titled("The scripts should be")
                .Ordered()
                .Grammar();
        }

        public IGrammar TheCssTagsShouldBe()
        {
            return VerifyStringList(() => _assetTagState.Styles.Select(x => x.AssetName()))
                .Titled("The styles should be")
                .Ordered()
                .Grammar();
        }
    }
}