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

        protected abstract T applyForwarder(object model, IChainForwarder forwarder);
        protected abstract T findAnswerFromResolver(object model, Func<IChainResolver, BehaviorChain> finder);


        protected T For(object model)
        {
            if (model == null) return null;

            var forwarder = resolver.FindForwarder(model);
            if (forwarder != null)
            {
                return applyForwarder(model, forwarder);
            }

            return findAnswerFromResolver(model, r => r.FindUnique(model));
        }

        protected T For(object model, string category)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var forwarder = resolver.FindForwarder(model, category);
            if (forwarder != null)
            {
                return applyForwarder(model, forwarder);
            }

            return findAnswerFromResolver(model, r => r.FindUnique(model, category));
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