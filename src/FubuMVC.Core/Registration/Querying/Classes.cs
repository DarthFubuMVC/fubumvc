using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration.Querying
{
    public class ChainForwarder
    {
        private readonly Func<object, IChainResolver, BehaviorChain> _forwarding;

        public ChainForwarder(Type inputType, Func<object, IChainResolver, BehaviorChain> forwarding)
        {
            _forwarding = forwarding;
            InputType = inputType;
        }

        public Type InputType { get; private set; }
        public string Category { get; set; }

        public BehaviorChain FindChain(IChainResolver resolver, object model)
        {
            throw new NotImplementedException();
        }
    }

    public interface IChainResolver
    {
        BehaviorChain Find<T>(Expression<Action<T>> expression);

        IEnumerable<BehaviorChain> Find(object model);
        BehaviorChain FindUnique(object model);
        BehaviorChain FindUnique(object model, string category);
    }

    public class ChainResolver : IChainResolver
    {
        private readonly ITypeResolver _typeResolver;
        private readonly BehaviorGraph _behaviorGraph;

        public ChainResolver(ITypeResolver typeResolver, BehaviorGraph behaviorGraph)
        {
            _typeResolver = typeResolver;
            _behaviorGraph = behaviorGraph;
        }

        public BehaviorChain Find<T>(Expression<Action<T>> expression)
        {
            var chain = _behaviorGraph.BehaviorFor(expression);
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
            return _behaviorGraph.Behaviors.Where(x => x.ActionInputType() == modelType);
        }


        public BehaviorChain FindUnique(object model)
        {
            var modelType = _typeResolver.ResolveType(model);

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
    }
}