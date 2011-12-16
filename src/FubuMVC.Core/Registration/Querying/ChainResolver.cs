using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration.Querying
{
    // TODO --- need to make this a singleton and add memoization here
    public class ChainResolver : IChainResolver
    {
        private readonly BehaviorGraph _behaviorGraph;
        private readonly ITypeResolver _typeResolver;

        public ChainResolver(ITypeResolver typeResolver, BehaviorGraph behaviorGraph)
        {
            _typeResolver = typeResolver;
            _behaviorGraph = behaviorGraph;
        }

        public BehaviorChain Find(Type handlerType, MethodInfo method, string category = null)
        {
            return Find(new ChainSearch{
                Type = handlerType,
                TypeMode = TypeSearchMode.HandlerOnly,
                MethodName = method.Name,
                CategoryOrHttpMethod = category
            });
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

            return Find(new ChainSearch{
                Type = modelType,
                TypeMode = TypeSearchMode.InputModelOnly,
                CategoryOrHttpMethod = category
            });
        }


        public BehaviorChain FindUniqueByInputType(Type modelType, string category = null)
        {
            return Find(new ChainSearch
            {
                Type = modelType,
                TypeMode = TypeSearchMode.Any,
                CategoryOrHttpMethod = category
            });
        }


        public BehaviorChain FindCreatorOf(Type type)
        {
            return _behaviorGraph.ChainThatCreates(type);
        }

        public void RootAt(string baseUrl)
        {
            _behaviorGraph.Behaviors.Where(x => x.Route != null).Each(x => x.Route.RootUrlAt(baseUrl));
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

        public BehaviorChain Find(ChainSearch search)
        {
            var candidates = search.FindCandidates(_behaviorGraph);

            var count = candidates.Count();
            switch (count)
            {
                case 1:
                    return candidates.Single();

                case 0:
                    throw new FubuException(2104, "No behavior chains are registered matching criteria:  " + search);

                default:
                    var message = "More than one behavior chain matching criteria:  " + search;
                    message += "\nMatches:";

                    candidates.Each(x =>
                    {
                        // TODO -- BehaviorChain needs a Description or a better ToString()

                        var description = "\n";
                        if (x.Route != null)
                        {
                            description += x.Route.Pattern + "  ";
                        }

                        if (x.FirstCall() != null)
                        {
                            description += " -- " + x.FirstCall().Description;
                        }

                        message += description;
                    });

                    throw new FubuException(2108, message);
            }
        }
    }
}