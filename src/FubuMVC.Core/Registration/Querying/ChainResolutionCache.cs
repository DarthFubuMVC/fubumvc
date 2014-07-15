using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration.Querying
{
    public class ChainResolutionCache : IChainResolver
    {
        private readonly Cache<ChainSearch, Func<BehaviorChain>> _results = new Cache<ChainSearch, Func<BehaviorChain>>();
        private readonly Cache<Type, BehaviorChain> _creators = new Cache<Type, BehaviorChain>();

        private readonly BehaviorGraph _behaviorGraph;
        private readonly ITypeResolver _typeResolver;

        public ChainResolutionCache(ITypeResolver typeResolver, BehaviorGraph behaviorGraph)
        {
            _typeResolver = typeResolver;
            _behaviorGraph = behaviorGraph;

            _results.OnMissing = find;

            _creators.OnMissing = type =>
            {
                return behaviorGraph.Behaviors.OfType<RoutedChain>().SingleOrDefault(x => x.UrlCategory.Creates.Contains(type));
            };
        }

        private Func<BehaviorChain> find(ChainSearch search)
        {
            var candidates = search.FindCandidates(_behaviorGraph);

            var count = candidates.Count();
            switch (count)
            {
                case 1:
                    var chain = candidates.Single();
                    return () => chain;

                case 0:
                    return () =>
                    {
                        throw new FubuException(2104, "No behavior chains are registered matching criteria:  " + search);
                    };

                default:
                    var message = "More than one behavior chain matching criteria:  " + search;
                    message += "\nMatches:";

                    candidates.Each(x =>
                    {
                        message += "\n" + x;
                    });

                    return () =>
                    {
                        throw new FubuException(2108, message);
                    };
            }
        }

        public BehaviorChain Find(ChainSearch search)
        {
            return _results[search]();
        }

        public BehaviorChain Find(Type handlerType, MethodInfo method, string category = null)
        {
            return Find(ChainSearch.ForMethod(handlerType, method, category));
        }

        public BehaviorChain FindUnique(object model, string category = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var forwarder = FindForwarder(model, category);
            if (forwarder != null)
            {
                return forwarder.FindChain(this, model).Chain;
            }

            var modelType = _typeResolver.ResolveType(model);

            var search = new ChainSearch
            {
                Type = modelType, TypeMode = TypeSearchMode.InputModelOnly, CategoryOrHttpMethod = category
            };

            return Find(search);
        }

        public BehaviorChain FindUniqueByType(Type modelType, string category = null)
        {
            return Find(ChainSearch.ByUniqueInputType(modelType, category));
        }

        public BehaviorChain FindCreatorOf(Type type)
        {
            return _creators[type];
        }

        public void RootAt(string baseUrl)
        {
            _behaviorGraph.Behaviors.OfType<RoutedChain>().Each(x =>
            {
                x.Route.RootUrlAt(baseUrl);
                x.AdditionalRoutes.Each(r => r.RootUrlAt(baseUrl));
            });
        }

        public IChainForwarder FindForwarder(object model, string category = null)
        {
            if (category == null)
            {
                var forwarder = FindForwarder(model, Categories.DEFAULT);
                if (forwarder != null) return forwarder;
            }

            var modelType = _typeResolver.ResolveType(model);
            return _behaviorGraph.Forwarders.SingleOrDefault(f => f.Category == category && f.InputType == modelType);
        }

        public void ClearAll()
        {
            _results.ClearAll();
            _creators.ClearAll();
        }
    }
}