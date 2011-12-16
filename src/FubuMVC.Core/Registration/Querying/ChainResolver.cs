using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration.Querying
{
    // TODO -- all exception throwing needs to be somewhere else
    public class ChainResolver : IChainResolver
    {
        private readonly ITypeResolver _typeResolver;
        private readonly BehaviorGraph _behaviorGraph;

        public ChainResolver(ITypeResolver typeResolver, BehaviorGraph behaviorGraph)
        {
            _typeResolver = typeResolver;
            _behaviorGraph = behaviorGraph;
        }

        public BehaviorChain Find(Type handlerType, MethodInfo method)
        {
            var chains = _behaviorGraph.ChainsFor(handlerType, method);
            if (chains.Count() > 1)
            {
                throw new FubuException(2108, "More than one behavior chain registered for {0}.{1}()", handlerType.FullName, method.Name);
            }


            var chain = chains.SingleOrDefault();
            if (chain == null)
            {
                throw new FubuException(2108, "No behavior chain registered for {0}.{1}()", handlerType.FullName, method.Name);
            }

            return chain;
        }


        private IEnumerable<BehaviorChain> findChainsByType(Type modelType)
        {
            return _behaviorGraph.ChainsFor(modelType);
        }


        // Will throw exceptions
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
            return FindUniqueByInputType(modelType, category);
        }




        public BehaviorChain FindUniqueByInputType(Type modelType, string category = null)
        {
            var chains = findChainsByType(modelType);

            if (chains.Count() == 0)
            {
                throw new FubuException(2102, "Unknown input type {0}", modelType.FullName);
            }


            var search = new ChainSearch{
                CategoryOrHttpMethod = category
            };

            chains = search.FindForCategory(chains);

            if (!chains.Any())
            {
                throw new FubuException(2104, "No behavior chains are registered for {0}, category {1}", modelType.FullName,
                                        category);
            }

            if (chains.Count() > 1)
            {
                if (category == null)
                {
                    throw new FubuException(2103,
                                            "More than one chain is registered for {0} and none is marked as the default.\nConsult the route/behavior diagnostics",
                                            modelType.FullName);
                }

                throw new FubuException(2105, "More than one behavior chain is registered for {0}, category {1}.\nConsult the diagnostics",
                                        modelType.FullName, category);
            }

            return chains.Single();
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
    }
}