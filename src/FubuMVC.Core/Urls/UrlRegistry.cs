using System;
using System.Collections.Generic;
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
            _templateFunc = (s) =>
            {
                return s.Replace("{", templatePattern.Start).Replace("}", templatePattern.End);
            };
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

        public string UrlFor<TInput>(RouteParameters parameters)
        {
            return UrlFor(typeof (TInput), parameters);
        }

        public string UrlFor(Type modelType, RouteParameters parameters)
        {
            var chain = resolver.FindUniqueByInputType(modelType);
            return chain.Route.Input.CreateUrlFromParameters(parameters);
        }

        public string UrlFor<TInput>(RouteParameters parameters, string category)
        {
            Type modelType = typeof (TInput);
            return UrlFor(modelType, category, parameters);
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

        private string buildUrlTemplate(object model, params Func<object, object>[] hash)
        {
            var chain = resolver.FindUnique(model);

            return _templateFunc(chain.Route.CreateTemplate(model, hash));
        }

        public string UrlForNew<T>()
        {
            return UrlForNew(typeof(T));
        }

        public string UrlForNew(Type entityType)
        {
            return ForNew(entityType);
        }

        public bool HasNewUrl<T>()
        {
            return HasNewUrl(typeof(T));
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
            
            // TODO -- throw if no input

            return model == null ? chain.Route.Pattern.ToAbsoluteUrl() : chain.Route.Input.CreateUrlFromInput(model);
        }
    }


}