using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Etags;

namespace FubuMVC.Core.Assets
{
    [Policy]
    public class AssetContentEndpoint : IConfigurationAction
    {
        public const string Pattern = "_content";

        public static bool Latched;

        public void Configure(BehaviorGraph graph)
        {
            if (Latched) return;

            // TODO -- Hokum.  Needs to be pluggable.
            var headerCache = graph.Services.DefaultServiceFor<IHeadersCache>().Value.As<IHeadersCache>();
            var chain = createAssetContentChain(graph);
            chain.Filters.Add(new EtagInvocationFilter(headerCache, args =>
            {
                var currentChain = args.Get<ICurrentChain>();
                return ResourceHash.For(currentChain);
            }));


            addCaching(chain);
            addWritingAction(chain);
        }

        private static void addWritingAction(BehaviorChain chain)
        {
            var call = ActionCall.For<AssetWriter>(x => x.Write(null));
            chain.AddToEnd(call);
        }

        private static void addCaching(BehaviorChain chain)
        {
            var cacheNode = new OutputCachingNode();

            chain.AddToEnd(cacheNode);
        }

        private static BehaviorChain createAssetContentChain(BehaviorGraph graph)
        {
            var chain = graph.AddChain();

            chain.Route = RouteBuilder.Build(typeof (AssetPath), Pattern);
            chain.Route.AddHttpMethodConstraint("GET");
            chain.Route.SessionStateRequirement = SessionStateRequirement.DoesNotUseSessionState;
            return chain;
        }
    }
}