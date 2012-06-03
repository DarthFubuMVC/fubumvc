using FubuCore;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Assets
{
    [Policy]
    public class AssetContentEndpoint : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            // TODO -- Hokum.  Needs to be pluggable.
            var assetCache = graph.Services.DefaultServiceFor<IAssetContentCache>().Value.As<AssetContentCache>();

            var chain = createAssetContentChain(graph);
            chain.Filters.Add(new AssetEtagInvocationFilter(assetCache));

            addCaching(chain, assetCache);
            addWritingAction(chain);
        }

        private static void addWritingAction(BehaviorChain chain)
        {
            var call = ActionCall.For<AssetWriter>(x => x.Write(null));
            chain.AddToEnd(call);
        }

        private static void addCaching(BehaviorChain chain, AssetContentCache assetCache)
        {
            var cacheNode = new OutputCachingNode
                            {
                                ETagCache = ObjectDef.ForValue(assetCache),
                                OutputCache = ObjectDef.ForValue(assetCache)
                            };

            chain.AddToEnd(cacheNode);
        }

        private static BehaviorChain createAssetContentChain(BehaviorGraph graph)
        {
            var chain = graph.AddChain();
            var pattern = "_content";
            chain.Route = RouteBuilder.Build(typeof(AssetPath), pattern);
            chain.Route.AddHttpMethodConstraint("GET");
            return chain;
        }
    }
}