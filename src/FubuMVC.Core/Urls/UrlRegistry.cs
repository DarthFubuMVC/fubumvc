using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuCore;

namespace FubuMVC.Core.Urls
{
    public class UrlRegistry : ChainInterrogator<string>, IUrlRegistry
    {
	    private readonly IChainUrlResolver _urlResolver;
	    private readonly ICurrentHttpRequest _httpRequest;
	    private readonly Func<string, string> _templateFunc;

        public UrlRegistry(IChainResolver resolver, IChainUrlResolver urlResolver, IUrlTemplatePattern templatePattern, ICurrentHttpRequest httpRequest)
            : base(resolver)
        {
	        _urlResolver = urlResolver;
	        _httpRequest = httpRequest;
	        _templateFunc = (s) => { return s.Replace("{", templatePattern.Start).Replace("}", templatePattern.End); };
        }

        public string UrlFor<TInput>(string categoryOrHttpMethod = null) where TInput : class
        {
            var type = typeof (TInput);
            try
            {
                if (type.IsConcreteWithDefaultCtor())
                {
                    var model = Activator.CreateInstance(type);
                    return For(model, categoryOrHttpMethod);
                }
            }


            catch (FubuException) // Yeah, I know
            {
                return For(type, null, categoryOrHttpMethod);
            }

            return For(type, null, categoryOrHttpMethod);
        }

        public string UrlFor(object model, string category = null)
        {
            return For(model, category);
        }

        public string UrlFor(Type modelType, RouteParameters parameters, string categoryOrHttpMethod = null)
        {
            var chain = resolver.FindUniqueByType(modelType, categoryOrHttpMethod);
	        return _urlResolver.UrlFor(chain, parameters);
        }

        public string UrlFor(Type handlerType, MethodInfo method, string categoryOrHttpMethod = null)
        {
            return For(handlerType, method, categoryOrHttpMethod);
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression, string categoryOrHttpMethod)
        {
            return UrlFor(typeof(TController), ReflectionHelper.GetMethod(expression), categoryOrHttpMethod);
        }

        public string TemplateFor(object model, string categoryOrHttpMethod = null)
        {
            return buildUrlTemplate(model, categoryOrHttpMethod, null);
        }

        public string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new()
        {
            return buildUrlTemplate(new TModel(), null, hash);
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
	        return _urlResolver.UrlFor(model, chain);
        }

        public string UrlFor<TInput>(RouteParameters parameters)
        {
            return UrlFor(typeof (TInput), parameters);
        }

        public string UrlFor<TInput>(RouteParameters parameters, string categoryOrHttpMethod = null)
        {
            var modelType = typeof (TInput);
            return UrlFor(modelType, parameters, categoryOrHttpMethod);
        }

        private string buildUrlTemplate(object model, string categoryOrHttpMethod, Func<object, object>[] hash)
        {
            var chain = resolver.FindUnique(model, categoryOrHttpMethod);

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
    }
}