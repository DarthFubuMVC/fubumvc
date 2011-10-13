using System.Collections.Generic;
using FubuMVC.Core.Diagnostics.Assets;
using FubuTestApplication;
using Serenity;
using Serenity.Endpoints;
using StoryTeller;
using StoryTeller.Engine;
using System.Linq;
using FubuCore;

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
            _endpoints = _driver.GetEndpointDriver();
        }

        public IGrammar RequestPageWithAssets()
        {
            return this["RequireFile"]
                .AsTable("Request a page with these required assets")
                .After(requestPage);
        }

        private AssetTagsState _assetTagState;
        private EndpointDriver _endpoints;

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

        [FormatAs("The asset names for combination {comboName} should be {names}")]
        public string[] AssetNamesInCombinationShouldBe(string comboName)
        {
            return _endpoints.ReadTextFrom(new AssetNamesRequest{
                Name = comboName
            }).ReadLines().ToArray();
        }

        [FormatAs("The asset sources for combination {comboName} should be {sources}")]
        public string[] AssetSourcesInCombinationShouldBe(string comboName)
        {
            return _endpoints.ReadTextFrom(new AssetSourcesRequest
            {
                Name = comboName
            }).ReadLines().ToArray();
        }
    }
}