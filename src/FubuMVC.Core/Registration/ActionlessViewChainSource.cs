using System;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using StructureMap.Graph.Scanning;

namespace FubuMVC.Core.Registration
{
    public class ActionlessViewChainSource : IChainSource
    {
        public Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            var types = TypeRepository.FindTypes(graph.ApplicationAssembly,
                TypeClassification.Closed | TypeClassification.Concretes, t => t.HasAttribute<ViewSubjectAttribute>());

            return types.ContinueWith(t =>
            {
                return t.Result.Select(type =>
                {
                    var chain = ChainForType(type);
                    chain.Tags.Add("ActionlessView");
                    chain.UrlCategory.Category = Categories.VIEW;

                    return chain;
                }).ToArray();
            });
        }

        public static BehaviorChain ChainForType(Type type)
        {
            if (type.HasAttribute<UrlPatternAttribute>())
            {
                var route = type.GetAttribute<UrlPatternAttribute>().BuildRoute(type);
                return new RoutedChain(route, type, type);
            }
            var chain = BehaviorChain.ForResource(type);
            chain.IsPartialOnly = true;

            return chain;
        }
    }
}