using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Policy;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Security;
using FubuMVC.Core.Urls;
using FubuCore;

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

    public class EndpointService : ChainInterrogator<Endpoint>, IEndpointService
    {
        private readonly IChainAuthorizor _authorizor;

        public EndpointService(IChainAuthorizor authorizor, IChainResolver resolver) : base(resolver)
        {
            _authorizor = authorizor;
        }

        protected override Endpoint createResult(object model, BehaviorChain chain)
        {
            return new Endpoint(){
                IsAuthorized = _authorizor.Authorize(chain, model) == AuthorizationRight.Allow,
                Url = chain.Route.CreateUrlFromInput(model).ToAbsoluteUrl()
            };
        }

        public Endpoint EndpointFor(object model)
        {
            return For(model);
        }

        public Endpoint EndpointFor(object model, string category)
        {
            return For(model, category);
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
            return ForNew(entityType);
        }

        public bool HasNewEndpoint<T>()
        {
            return HasNewEndpoint(typeof (T));
        }

        public bool HasNewEndpoint(Type type)
        {
            return HasNew(type);
        }

        public Endpoint EndpointFor(Type handlerType, MethodInfo method)
        {
            return For(handlerType, method);
        }
    }
}