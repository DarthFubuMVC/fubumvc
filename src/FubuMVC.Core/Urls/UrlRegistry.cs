using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuCore;

namespace FubuMVC.Core.Urls
{
    public class UrlRegistry : ChainInterrogator<string>, IUrlRegistry
    {
        public static readonly string AssetsUrlFolder = "_content";

        private readonly ICurrentHttpRequest _httpRequest;
        private readonly Func<string, string> _templateFunc;

        public UrlRegistry(IChainResolver resolver, IUrlTemplatePattern templatePattern, ICurrentHttpRequest httpRequest)
            : base(resolver)
        {
            _httpRequest = httpRequest;
            _templateFunc = (s) => { return s.Replace("{", templatePattern.Start).Replace("}", templatePattern.End); };
        }

        public string UrlFor<TInput>() where TInput : class, new()
        {
            return For(new TInput());
        }

        public string UrlFor(object model, string category = null)
        {
            return For(model, category);
        }

        public string UrlFor(Type modelType, RouteParameters parameters)
        {
            var chain = resolver.FindUniqueByInputType(modelType);
            string url = chain.Route.Input.CreateUrlFromParameters(parameters);

            return _httpRequest.ToFullUrl(url);
        }

        public string UrlFor(Type modelType, string category, RouteParameters parameters)
        {
            var chain = resolver.FindUniqueByInputType(modelType, category);
            string url = chain.Route.Input.CreateUrlFromParameters(parameters);

            return _httpRequest.ToFullUrl(url);
        }

        public string UrlForAsset(AssetFolder? folder, string name)
        {
            var relativeUrl = DetermineRelativeAssetUrl(folder, name);
            return _httpRequest.ToFullUrl(relativeUrl);
        }

        public string UrlFor(Type handlerType, MethodInfo method)
        {
            return For(handlerType, method);
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression)
        {
            return UrlFor(typeof(TController), ReflectionHelper.GetMethod(expression));
        }

        public string TemplateFor(object model)
        {
            return buildUrlTemplate(model, null);
        }

        public string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new()
        {
            return buildUrlTemplate(new TModel(), hash);
        }

        public string UrlForNew(Type entityType)
        {
            return forNew(entityType);
        }


        public bool HasNewUrl(Type type)
        {
            return resolver.FindCreatorOf(type) != null;
        }



        protected override string createResult(object model, BehaviorChain chain)
        {
            string urlFromInput = chain.Route.CreateUrlFromInput(model);
            return _httpRequest.ToFullUrl(urlFromInput);
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

            string url = _templateFunc(chain.Route.CreateTemplate(model, hash));
            return _httpRequest.ToFullUrl(url);
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

        // TODO -- move the unit tests
        public static string DetermineRelativeAssetUrl(IAssetTagSubject subject)
        {
            var folder = subject.Folder;
            var name = subject.Name;

            return DetermineRelativeAssetUrl(folder, name);
        }

        // TODO -- move the unit tests
        public static string DetermineRelativeAssetUrl(AssetFolder? folder, string name)
        {
            return "{0}/{1}/{2}".ToFormat(AssetsUrlFolder, folder, name);
        }
    }
}