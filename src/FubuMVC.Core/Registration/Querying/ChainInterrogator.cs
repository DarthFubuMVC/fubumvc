using System;
using System.Reflection;
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

        private T processForwarder(object model, Func<IChainForwarder> forwarderSource, Func<IChainResolver, BehaviorChain> finder)
        {
            var forwarder = forwarderSource();
            if (forwarder != null)
            {
                var result = forwarder.FindChain(resolver, model);
                return createResult(result.RealInput, result.Chain);
            }

            return findAnswerFromResolver(model, finder);
        }

        protected T For(object model)
        {
            if (model == null) return null;

            return processForwarder(model, () => resolver.FindForwarder(model), r => r.FindUnique(model));
        }

        protected T For(object model, string category)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return processForwarder(
                model, 
                () => resolver.FindForwarder(model, category),
                r => r.FindUnique(model, category));
        }

        protected T For(Type handlerType, MethodInfo method)
        {
            return findAnswerFromResolver(null, r =>
            {
                var chain = r.Find(handlerType, method);
                if (chain == null)
                {
                    throw new FubuException(2108, "No behavior chain registered for {0}.{1}()", handlerType.FullName, method.Name);
                }

                return chain;
            });
        }

        protected bool HasNew(Type entityType)
        {
            return resolver.FindCreatorOf(entityType) != null;
        }

        protected T ForNew(Type entityType)
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