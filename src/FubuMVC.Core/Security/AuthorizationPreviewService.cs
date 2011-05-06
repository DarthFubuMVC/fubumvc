using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Security
{
    public class AuthorizationPreviewService : ChainInterrogator<AuthorizationRight>, IAuthorizationPreviewService
    {
        private readonly ITypeResolver _types;
        private readonly IEndPointAuthorizorFactory _factory;
        private readonly IFubuRequest _request;

        public AuthorizationPreviewService(ITypeResolver types, IChainResolver resolver, IEndPointAuthorizorFactory factory, IFubuRequest request) : base(resolver)
        {
            _types = types;
            _factory = factory;
            _request = request;
        }

        protected override AuthorizationRight createResult(object model, BehaviorChain chain)
        {
            if (model != null)
            {
                _request.Set(_types.ResolveType(model), model);
            }

            return rightsFor(chain);
        }

        private AuthorizationRight rightsFor(BehaviorChain chain)
        {
            var endpoint = _factory.AuthorizorFor(chain.UniqueId);
            return endpoint.IsAuthorized(_request);
        }

        public bool IsAuthorized(object model)
        {
            return For(model) == AuthorizationRight.Allow;
        }

        public bool IsAuthorized(object model, string category)
        {
            return For(model, category) == AuthorizationRight.Allow;
        }

        public bool IsAuthorized<TController>(Expression<Action<TController>> expression)
        {
            return IsAuthorized(typeof (TController), ReflectionHelper.GetMethod(expression));
        }

        public bool IsAuthorizedForNew<T>()
        {
            return IsAuthorizedForNew(typeof (T));
        }

        public bool IsAuthorizedForNew(Type entityType)
        {
            return ForNew(entityType) == AuthorizationRight.Allow;
        }

        public bool IsAuthorizedForPropertyUpdate(object model)
        {
            return IsAuthorized(model, Categories.PROPERTY_EDIT);
        }

        [Obsolete("TEMPORARY HACK")]
        public bool IsAuthorizedForPropertyUpdate(Type type)
        {
            object o = Activator.CreateInstance(type);
            return IsAuthorizedForPropertyUpdate(o);
        }

        public bool IsAuthorized(Type handlerType, MethodInfo method)
        {
            return For(handlerType, method) == AuthorizationRight.Allow;
        }
    }
}