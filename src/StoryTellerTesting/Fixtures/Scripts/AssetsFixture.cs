using System.Collections.Generic;
using System.Net;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics.Assets;
using FubuTestApplication;
using Serenity;
using Serenity.Endpoints;
using StoryTeller;
using StoryTeller.Assertions;
using StoryTeller.Engine;
using System.Linq;
using FubuCore;

namespace IntegrationTesting.Fixtures.Scripts
{
    public class AssetsFixture : Fixture
    {
        private Cache<string, string> _assetContents;
        private Serenity.NavigationDriver _driver;

        public IGrammar IfTheAssetsAre()
        {
            return Embed<AssetPipelineSetupFixture>("If the asset pipeline is configured as");
        }

        public override void SetUp(ITestContext context)
        {
            _driver = context.Retrieve<Serenity.NavigationDriver>();
            _endpoints = _driver.GetEndpointDriver();

            _assetContents = new Cache<string, string>(file =>
            {
                var url = _driver.AssetUrlFor(file);
                return new WebClient().DownloadString(url);
            });
        }


        private AssetTagsState _assetTagState;
        private EndpointDriver _endpoints;

        [FormatAs("Request a page that requires assets {names}")]
        public void RequestPageWithAssets(string[] names)
        {
            var request = new ScriptRequest
            {
                Mandatories = names.Join(",")
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
            return _endpoints.ReadTextFrom(new AssetNamesRequest
            {
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

        [FormatAs("Asset {assetName} should contain the text {content}")]
        public bool AssetContainsText(string assetName, string content)
        {
            var allText = _assetContents[assetName];
            if (allText.Contains(content)) return true;

            StoryTellerAssert.Fail(allText);

            return false;
        }
    }
}