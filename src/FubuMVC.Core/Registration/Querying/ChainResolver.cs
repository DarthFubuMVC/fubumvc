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
    public class ChainResolver : IChainResolver
    {
        private readonly ITypeResolver _typeResolver;
        private readonly BehaviorGraph _behaviorGraph;

        public ChainResolver(ITypeResolver typeResolver, BehaviorGraph behaviorGraph)
        {
            _typeResolver = typeResolver;
            _behaviorGraph = behaviorGraph;
        }

        // TODO -- This really needs to change to returning an IEnumerable and
        // we possibly need an alternative that finds by category
        public BehaviorChain Find<T>(Expression<Action<T>> expression)
        {
            var chain = _behaviorGraph.ChainsFor(typeof(T), ReflectionHelper.GetMethod(expression)).SingleOrDefault();
            if (chain == null)
            {
                throw new FubuException(2108, "No behavior chain registered for {0}.{1}()", typeof(T).FullName, ReflectionHelper.GetMethod(expression).Name);
            }

            return chain;
        }

        public IEnumerable<BehaviorChain> Find(object model)
        {
            var modelType = _typeResolver.ResolveType(model);
            return findChainsByType(modelType);
        }

        private IEnumerable<BehaviorChain> findChainsByType(Type modelType)
        {
            return _behaviorGraph.ChainsFor(modelType);
        }


        public BehaviorChain FindUnique(object model)
        {
            var forwarder = FindForwarder(model);
            if (forwarder != null)
            {
                return forwarder.FindChain(this, model).Chain;
            }
            
            var modelType = _typeResolver.ResolveType(model);
            

            return FindUniqueByInputType(modelType);
        }

        public BehaviorChain FindUniqueByInputType(Type modelType)
        {
            var chains = findChainsByType(modelType);
            switch (chains.Count())
            {
                case 0:
                    throw new FubuException(2102, "Unknown input type {0}", modelType.FullName);

                case 1:
                    return chains.Single();

                default:
                    var defaultChain = chains.FirstOrDefault(x => x.UrlCategory.Category == Categories.DEFAULT);
                    if (defaultChain == null)
                    {
                        if (chains.Count(x => x.UrlCategory.Category.IsEmpty()) == 1)
                        {
                            defaultChain = chains.First(x => x.UrlCategory.Category.IsEmpty());
                        }
                    }

                    if (defaultChain != null)
                    {
                        return defaultChain;
                    }

                    throw new FubuException(2103,
                                            "More than one chain is registered for {0} and none is marked as the default.\nConsult the route/behavior diagnostics",
                                            modelType.FullName);
            }
        }

        // Will throw exceptions
        public BehaviorChain FindUnique(object model, string category)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var modelType = _typeResolver.ResolveType(model);
            return FindUniqueByInputType(modelType, category);
        }

        public BehaviorChain FindUniqueByInputType(Type modelType, string category)
        {
            var chains = findChainsByType(modelType).Where(x => x.UrlCategory.Category == category);

            if (!chains.Any())
                throw new FubuException(2104, "No behavior chains are registered for {0}, category {1}", modelType.FullName,
                                        category);
            if (chains.Count() > 1)
            {
                throw new FubuException(2105, "More than one behavior chain is registered for {0}, category {1}.\nConsult the diagnostics",
                                        modelType.FullName, category);
            }

            return chains.Single();
        }

        public BehaviorChain Find(Type handlerType, MethodInfo method)
        {
            return _behaviorGraph.ChainsFor(handlerType, method).SingleOrDefault();
        }

        public BehaviorChain FindCreatorOf(Type type)
        {
            return _behaviorGraph.ChainThatCreates(type);
        }

        public void RootAt(string baseUrl)
        {
            _behaviorGraph.Behaviors.Where(x => x.Route != null).Each(x => x.Route.RootUrlAt(baseUrl));
        }

        public IChainForwarder FindForwarder(object model, string category)
        {
            var modelType = _typeResolver.ResolveType(model);
            return _behaviorGraph.Forwarders.SingleOrDefault(f => f.Category == category && f.InputType == modelType);
        }

        public IChainForwarder FindForwarder(object model)
        {
            return FindForwarder(model, Categories.DEFAULT) ?? FindForwarder(model, null);   
        }
    }
}