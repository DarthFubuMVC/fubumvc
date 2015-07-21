using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Runtime
{
    /// <summary>
    /// Combines the functionality of IUrlRegistry and IAuthorizationPreviewService to both resolve Url's and 
    /// test authorization rules for a behavior chain / endpoint in the system
    /// </summary>
    public interface IEndpointService
    {
        /// <summary>
        /// Find an Endpoint for the given input model and category or http method
        /// </summary>
        /// <param name="model"></param>
        /// <param name="categoryOrHttpMethod"></param>
        /// <returns></returns>
        Endpoint EndpointFor(object model, string categoryOrHttpMethod = null);

        /// <summary>
        /// Find an Endpoint for the given handler type, method, and category/http method
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="expression"></param>
        /// <param name="categoryOrHttpMethod"></param>
        /// <returns></returns>
        Endpoint EndpointFor<TController>(Expression<Action<TController>> expression, string categoryOrHttpMethod = null);

        /// <summary>
        /// Finds the endpoint that would create a new instance of entityType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Endpoint EndpointForNew<T>();

        /// <summary>
        /// Finds the endpoint that would create a new instance of entityType
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        Endpoint EndpointForNew(Type entityType);
        bool HasNewEndpoint<T>();
        bool HasNewEndpoint(Type type);

        /// <summary>
        /// Find an Endpoint for the given handler type, method, and category or http method
        /// </summary>
        /// <param name="handlerType"></param>
        /// <param name="method"></param>
        /// <param name="categoryOrHttpMethod"></param>
        /// <returns></returns>
        Endpoint EndpointFor(Type handlerType, MethodInfo method, string categoryOrHttpMethod = null);
    }

    public class Endpoint
    {
        public string Url { get; set; }
        public bool IsAuthorized { get; set; }

        public BehaviorChain Chain { get; set; }

        public bool Equals(Endpoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Url, Url) && other.IsAuthorized.Equals(IsAuthorized);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Endpoint)) return false;
            return Equals((Endpoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Url != null ? Url.GetHashCode() : 0)*397) ^ IsAuthorized.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("Url: {0}, IsAuthorized: {1}", Url, IsAuthorized);
        }
    }

    public class EndpointService : ChainInterrogator<Endpoint>, IEndpointService
    {
        private readonly IChainAuthorizor _authorizor;
	    private readonly IChainUrlResolver _urlResolver;

        public EndpointService(IChainAuthorizor authorizor, IChainResolver resolver, IChainUrlResolver urlResolver)
            : base(resolver)
        {
	        _authorizor = authorizor;
	        _urlResolver = urlResolver;
        }

	    public Endpoint EndpointFor(object model, string categoryOrHttpMethod = null)
        {
            return For(model, categoryOrHttpMethod);
        }

        public Endpoint EndpointFor<TController>(Expression<Action<TController>> expression,
                                                 string categoryOrHttpMethod = null)
        {
            return EndpointFor(typeof (TController), ReflectionHelper.GetMethod(expression), categoryOrHttpMethod);
        }

        public Endpoint EndpointForNew<T>()
        {
            return EndpointForNew(typeof (T));
        }

        public Endpoint EndpointForNew(Type entityType)
        {
            return forNew(entityType);
        }

        public bool HasNewEndpoint<T>()
        {
            return HasNewEndpoint(typeof (T));
        }

        public bool HasNewEndpoint(Type type)
        {
            return hasNew(type);
        }

        public Endpoint EndpointFor(Type handlerType, MethodInfo method, string categoryOrHttpMethod = null)
        {
            return For(handlerType, method, categoryOrHttpMethod);
        }

        protected override Endpoint createResult(object model, BehaviorChain chain)
        {
	        var url = _urlResolver.UrlFor(model, chain);
            return new Endpoint{
                IsAuthorized = _authorizor.Authorize(chain, model) == AuthorizationRight.Allow,
                Url = url,
                Chain = chain
            };
        }
    }
}