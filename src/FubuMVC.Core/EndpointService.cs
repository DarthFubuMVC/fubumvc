using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Security;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core
{
    public interface IEndpointService
    {
        Endpoint EndpointFor(object model);
        Endpoint EndpointFor(object model, string category);
        Endpoint EndpointFor<TController>(Expression<Action<TController>> expression);

        Endpoint EndpointForNew<T>();
        Endpoint EndpointForNew(Type entityType);
        bool HasNewEndpoint<T>();
        bool HasNewEndpoint(Type type);


        // Not sure these two methods won't get axed
        Endpoint EndpointForPropertyUpdate(object model);
        Endpoint EndpointForPropertyUpdate(Type type);

        Endpoint EndpointFor(Type handlerType, MethodInfo method);
    }

    public class Endpoint
    {
        public string Url { get; set; }
        public bool IsAuthorized { get; set; }

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

    public class EndpointService : IEndpointService
    {
        private readonly IAuthorizationPreviewService _authorization;
        private readonly IUrlRegistry _urls;

        public EndpointService(IAuthorizationPreviewService authorization, IUrlRegistry urls)
        {
            _authorization = authorization;
            _urls = urls;
        }

        public Endpoint EndpointFor(object model)
        {
            return new Endpoint(){
                IsAuthorized = _authorization.IsAuthorized(model),
                Url = _urls.UrlFor(model)
            };
        }

        public Endpoint EndpointFor(object model, string category)
        {
            return new Endpoint()
            {
                IsAuthorized = _authorization.IsAuthorized(model, category),
                Url = _urls.UrlFor(model, category)
            };
        }

        public Endpoint EndpointFor<TController>(Expression<Action<TController>> expression)
        {
            return EndpointFor(typeof (TController), ReflectionHelper.GetMethod(expression));
        }

        public Endpoint EndpointForNew<T>()
        {
            return EndpointForNew(typeof (T));
        }

        public Endpoint EndpointForNew(Type entityType)
        {
            return new Endpoint(){
                IsAuthorized = _authorization.IsAuthorizedForNew(entityType),
                Url = _urls.UrlForNew(entityType)
            };
        }

        public bool HasNewEndpoint<T>()
        {
            return HasNewEndpoint(typeof (T));
        }

        public bool HasNewEndpoint(Type type)
        {
            return _urls.HasNewUrl(type);
        }

        public Endpoint EndpointForPropertyUpdate(object model)
        {
            return new Endpoint(){
                IsAuthorized = _authorization.IsAuthorizedForPropertyUpdate(model),
                Url = _urls.UrlForPropertyUpdate(model)
            };
        }

        public Endpoint EndpointForPropertyUpdate(Type type)
        {
            return new Endpoint()
            {
                IsAuthorized = _authorization.IsAuthorizedForPropertyUpdate(type),
                Url = _urls.UrlForPropertyUpdate(type)
            };
        }

        public Endpoint EndpointFor(Type handlerType, MethodInfo method)
        {
            return new Endpoint(){
                IsAuthorized = _authorization.IsAuthorized(handlerType, method),
                Url = _urls.UrlFor(handlerType, method)
            };
        }
    }
}