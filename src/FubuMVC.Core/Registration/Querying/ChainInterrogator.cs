using System;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Querying
{
    public abstract class ChainInterrogator<T> where T : class
    {
        private readonly IChainResolver _resolver;

        protected ChainInterrogator(IChainResolver resolver)
        {
            _resolver = resolver;
        }

        protected IChainResolver resolver
        {
            get { return _resolver; }
        }

        protected abstract T createResult(object model, BehaviorChain chain);
        protected T findAnswerFromResolver(object model, Func<IChainResolver, BehaviorChain> finder)
        {
            var chain = finder(resolver);
            return createResult(model, chain);
        }

        protected T For(object model, string categoryOrMethod = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return findAnswerFromResolver(model, r => r.FindUnique(model, categoryOrMethod));
        }

        protected T For(Type handlerType, MethodInfo method, string categoryOrHttpMethod = null)
        {
            return findAnswerFromResolver(null, r =>
            {
                var chain = r.Find(handlerType, method, categoryOrHttpMethod);
                if (chain == null)
                {
                    throw new FubuException(2108, "No behavior chain registered for {0}.{1}()", handlerType.FullName, method.Name);
                }

                return chain;
            });
        }

        protected bool hasNew(Type entityType)
        {
            return resolver.FindCreatorOf(entityType) != null;
        }

        protected T forNew(Type entityType)
        {
            return findAnswerFromResolver(null, r =>
            {
                var chain = r.FindCreatorOf(entityType);

                if (chain == null)
                {
                    throw new FubuException(2109, "No 'new' route exists for type {0}", entityType.FullName);
                }

                return chain;
            });
        }

    }
}