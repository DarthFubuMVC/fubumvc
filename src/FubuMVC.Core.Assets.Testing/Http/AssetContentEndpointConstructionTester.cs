using System.Linq;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.PathBased;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class AssetContentEndpointConstructionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var behaviorGraph = BehaviorGraph.BuildFrom(x => x.Import<AssetBottleRegistration>());

            theChain = behaviorGraph.BehaviorFor<AssetWriter>(x => x.Write(null));
        }

        #endregion

        private BehaviorChain theChain;
        private AssetContentCache theContentCache;

        [Test]
        public void the_chain_should_absolutely_not_require_session_state()
        {
            theChain.Route.SessionStateRequirement.ShouldEqual(SessionStateRequirement.DoesNotUseSessionState);
        }

        [Test]
        public void generates_url_with_asset_path_of_different_lengths()
        {
            theChain.Route.CreateUrlFromInput(new AssetPath("scripts/file1.js")).ShouldEqual("_content/scripts/file1.js");
            theChain.Route.CreateUrlFromInput(new AssetPath("scripts/f1/file1.js")).ShouldEqual(
                "_content/scripts/f1/file1.js");
            theChain.Route.CreateUrlFromInput(new AssetPath("scripts/f1/f2/file1.js")).ShouldEqual(
                "_content/scripts/f1/f2/file1.js");
            theChain.Route.CreateUrlFromInput(new AssetPath("scripts/f1/f2/f3/file1.js")).ShouldEqual(
                "_content/scripts/f1/f2/f3/file1.js");
        }

        [Test]
        public void route_pattern_should_be_the_same()
        {
            theChain.Route.Pattern.ShouldEqual("_content/" + ResourcePath.UrlSuffix);
        }

        [Test]
        public void the_chain_exists()
        {
            theChain.ShouldNotBeNull();
        }

        [Test]
        public void the_chain_should_have_an_etag_invocation_filter()
        {
            theChain.Filters.Single().ShouldBeOfType<EtagInvocationFilter>();
        }
    }
}