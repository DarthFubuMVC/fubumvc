using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuCore;

namespace FubuMVC.Core.Urls
{
    public class UrlRegistry : ChainInterrogator<string>, IUrlRegistry
    {
        private readonly Func<string, string> _templateFunc;

        public UrlRegistry(IChainResolver resolver, IUrlTemplatePattern templatePattern)
            : base(resolver)
        {
            _templateFunc = (s) => { return s.Replace("{", templatePattern.Start).Replace("}", templatePattern.End); };
        }

        public string UrlFor(object model)
        {
            return For(model);
        }

        public string UrlFor(object model, string category)
        {
            return For(model, category);
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression)
        {
            return findAnswerFromResolver(null, r => r.Find(expression));
        }

        public string UrlFor(Type modelType, RouteParameters parameters)
        {
            var chain = resolver.FindUniqueByInputType(modelType);
            return chain.Route.Input.CreateUrlFromParameters(parameters);
        }

        public string UrlFor(Type modelType, string category, RouteParameters parameters)
        {
            var chain = resolver.FindUniqueByInputType(modelType, category);
            return chain.Route.Input.CreateUrlFromParameters(parameters);
        }

        public string UrlFor(Type handlerType, MethodInfo method)
        {
            return For(handlerType, method);
        }

        public string TemplateFor(object model)
        {
            return buildUrlTemplate(model, null);
        }

        public string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new()
        {
            return buildUrlTemplate(new TModel(), hash);
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
            var o = Activator.CreateInstance(type);
            return UrlForPropertyUpdate(o);
        }

        protected override string createResult(object model, BehaviorChain chain)
        {
            return chain.Route.CreateUrlFromInput(model).ToAbsoluteUrl();
        }

        public string UrlFor<TInput>(RouteParameters parameters)
        {
            return UrlFor(typeof (TInput), parameters);
        }

        public string UrlFor<TInput>(RouteParameters parameters, string category)
        {
            var modelType = typeof (TInput);
            return UrlFor(modelType, category, parameters);
        }

        private string buildUrlTemplate(object model, params Func<object, object>[] hash)
        {
            var chain = resolver.FindUnique(model);

            return _templateFunc(chain.Route.CreateTemplate(model, hash));
        }

        /// <summary>
        ///   This is only for automated testing scenarios.  Do NOT use in real
        ///   scenarios
        /// </summary>
        /// <param name = "baseUrl"></param>
        public void RootAt(string baseUrl)
        {
            resolver.RootAt(baseUrl);
        }
    }
}