using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.Urls
{
    public class UrlRegistry : IUrlRegistry
    {
        private readonly IChainResolver _resolver;

        public UrlRegistry(IChainResolver resolver)
        {
            _resolver = resolver;
        }

        [Obsolete]
        public void Forward<TInput>(Expression<Func<TInput, IUrlRegistry, string>> forward)
        {
            Forward(Categories.DEFAULT, forward);
        }

        [Obsolete]
        public void Forward<TInput>(Type type, string category, Expression<Func<TInput, IUrlRegistry, string>> forward)
        {
            //Func<object, string> func = o => forward.Compile()((TInput) o, this);
            //var url = new ForwardUrl(type, func, category, forward.ToString());
            //AddModel(url);
        }

        [Obsolete]
        public void Forward<TInput>(string category, Expression<Func<TInput, IUrlRegistry, string>> forward)
        {
            Forward(typeof (TInput), category, forward);
        }

        public string UrlFor(object model)
        {
            if (model == null) return null;

            var forwarder = _resolver.FindForwarder(model);
            if (forwarder != null)
            {
                return forwarder.FindUrl(_resolver, model);
            }

            return returnUrl(model, r => r.FindUnique(model));
        }

        public string UrlFor(object model, string category)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var forwarder = _resolver.FindForwarder(model, category);
            if (forwarder != null)
            {
                return forwarder.FindUrl(_resolver, model);
            }

            return returnUrl(model, r => r.FindUnique(model, category));
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression)
        {
            return returnUrl(null, r => r.Find(expression));
        }

        public string UrlFor(Type handlerType, MethodInfo method)
        {
            return returnUrl(null, r =>
            {
                var chain = r.Find(handlerType, method);
                if (chain == null)
                {
                    throw new FubuException(2108, "No behavior chain registered for {0}.{1}()", handlerType.FullName, method.Name);
                }

                return chain;
            });
        }

        public string UrlForNew<T>()
        {
            return UrlForNew(typeof (T));
        }

        public string UrlForNew(Type entityType)
        {
            return returnUrl(null, r =>
            {
                var chain = r.FindCreatorOf(entityType);

                if (chain == null)
                {
                    throw new FubuException(2109, "No 'new' route exists for type {0}", entityType.FullName);
                }

                return chain;
            });
        }

        public bool HasNewUrl<T>()
        {
            return HasNewUrl(typeof (T));
        }

        public bool HasNewUrl(Type type)
        {
            return _resolver.FindCreatorOf(type) != null;
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
            _resolver.RootAt(baseUrl);
        }

        private string returnUrl(object model, Func<IChainResolver, BehaviorChain> finder)
        {
            BehaviorChain chain = finder(_resolver);
            return chain.Route.CreateUrl(model);
        }
    }
}