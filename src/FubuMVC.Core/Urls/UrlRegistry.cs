using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.Urls
{
    public class UrlRegistry : ChainInterrogator<string>, IUrlRegistry
    {
        public UrlRegistry(IChainResolver resolver) : base(resolver)
        {
        }

        public string UrlFor(object model)
        {
            return For(model);
        }

        protected override string applyForwarder(object model, IChainForwarder forwarder)
        {
            return forwarder.FindUrl(resolver, model);
        }

        public string UrlFor(object model, string category)
        {
            return For(model, category);
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression)
        {
            return findAnswerFromResolver(null, r => r.Find(expression));
        }

        public string UrlFor(Type handlerType, MethodInfo method)
        {
            return For(handlerType, method);
        }

        public string UrlForNew<T>()
        {
            return UrlForNew(typeof (T));
        }

        public string UrlForNew(Type entityType)
        {
            return ForNew(entityType);
        }

        public bool HasNewUrl<T>()
        {
            return HasNewUrl(typeof (T));
        }

        public bool HasNewUrl(Type type)
        {
            return resolver.FindCreatorOf(type) != null;
        }

        public string UrlForPropertyUpdate(object model)
        {
            return UrlFor(model, Categories.PROPERTY_EDIT);
        }

        [Obsolete("TEMPORARY HACK")]
        public string UrlForPropertyUpdate(Type type)
        {
            object o = Activator.CreateInstance(type);
            return UrlForPropertyUpdate(o);
        }

        /// <summary>
        /// This is only for automated testing scenarios.  Do NOT use in real
        /// scenarios
        /// </summary>
        /// <param name="baseUrl"></param>
        public void RootAt(string baseUrl)
        {
            resolver.RootAt(baseUrl);
        }

        protected override string findAnswerFromResolver(object model, Func<IChainResolver, BehaviorChain> finder)
        {
            BehaviorChain chain = finder(resolver);
            return chain.Route.CreateUrl(model);
        }
    }
}