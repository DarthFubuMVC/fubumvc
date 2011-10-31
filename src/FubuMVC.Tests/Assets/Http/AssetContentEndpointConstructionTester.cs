using FubuMVC.Core;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Resources.PathBased;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class AssetContentEndpointConstructionTester
    {
        private BehaviorChain theChain;
        private AssetContentCache theContentCache;

        [SetUp]
        public void SetUp()
        {
            BehaviorGraph behaviorGraph = new FubuRegistry().BuildGraph();

            behaviorGraph.Behaviors.Count().ShouldEqual(1);

            theContentCache = behaviorGraph.Services.DefaultServiceFor<IAssetContentCache>()
                .Value.ShouldBeOfType<AssetContentCache>();

            theChain = behaviorGraph.BehaviorFor<AssetWriter>(x => x.Write(null));
        }

        [Test]
        public void the_chain_exists()
        {
            theChain.ShouldNotBeNull();
        }

        [Test]
        public void route_pattern_should_be_the_same()
        {
            theChain.Route.Pattern.ShouldEqual("_content/" + ResourcePath.UrlSuffix);
        }

        [Test]
        public void generates_url_with_asset_path_of_different_lengths()
        {
            theChain.Route.CreateUrlFromInput(new AssetPath("scripts/file1.js")).ShouldEqual("_content/scripts/file1.js");
            theChain.Route.CreateUrlFromInput(new AssetPath("scripts/f1/file1.js")).ShouldEqual("_content/scripts/f1/file1.js");
            theChain.Route.CreateUrlFromInput(new AssetPath("scripts/f1/f2/file1.js")).ShouldEqual("_content/scripts/f1/f2/file1.js");
            theChain.Route.CreateUrlFromInput(new AssetPath("scripts/f1/f2/f3/file1.js")).ShouldEqual("_content/scripts/f1/f2/f3/file1.js");
        }

        [Test]
        public void first_behavior_should_be_the_IfNoneMatchNode()
        {
            theChain.First().ShouldBeOfType<IfNoneMatchNode>()
                .HandlerType.ShouldEqual(typeof(ETagHandler<AssetPath>));
        }

        [Test]
        public void the_etag_cache_should_be_an_AssetContentCache()
        {
            var etagCache = theChain.First().ShouldBeOfType<IfNoneMatchNode>()
                .HandlerDef.FindDependencyValueFor<IEtagCache>();


            etagCache
                .ShouldNotBeNull()
                .ShouldBeOfType<AssetContentCache>();
        }

        [Test]
        public void should_apply_a_caching_node_before_the_action_that_uses_the_asset_content_cache()
        {
            var cachingNode = theChain.FirstCall().Previous.ShouldBeOfType<OutputCachingNode>();
            cachingNode.ETagCache.Value.ShouldBeOfType<AssetContentCache>();
            cachingNode.OutputCache.Value.ShouldBeOfType<AssetContentCache>();
        }

        [Test]
        public void same_asset_content_cache_should_be_used_everywhere()
        {
            var cachingNode = theChain.FirstCall().Previous.ShouldBeOfType<OutputCachingNode>();
            cachingNode.ETagCache.Value.ShouldBeTheSameAs(theContentCache);
            cachingNode.OutputCache.Value.ShouldBeTheSameAs(theContentCache);

            var etagCache = theChain.First().ShouldBeOfType<IfNoneMatchNode>()
                .HandlerDef.FindDependencyValueFor<IEtagCache>();

            etagCache.ShouldBeTheSameAs(theContentCache);
        }

        [Test]
        public void should_have_a_header_handler()
        {
            theChain.OfType<Process>().Any(x => x.BehaviorType == typeof (WriteHeadersBehavior))
                .ShouldBeTrue();
        }

        [Test]
        public void there_is_a_fubu_continuation_handler_right_after_the_etag_handler()
        {
            theChain.First().ShouldBeOfType<IfNoneMatchNode>()
                .Next.ShouldBeOfType<ContinuationNode>();
        }
    }
}