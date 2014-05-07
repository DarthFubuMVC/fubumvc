using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.Security
{
    public class AuthorizationPreviewService : ChainInterrogator<AuthorizationRight>, IAuthorizationPreviewService
    {
        private readonly IChainAuthorizor _authorizor;

        public AuthorizationPreviewService(IChainResolver resolver, IChainAuthorizor authorizor) : base(resolver)
        {
            _authorizor = authorizor;
        }

        protected override AuthorizationRight createResult(object model, BehaviorChain chain)
        {
            return _authorizor.Authorize(chain, model);
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
            return forNew(entityType) == AuthorizationRight.Allow;
        }

        public bool IsAuthorized(Type handlerType, MethodInfo method)
        {
            return For(handlerType, method) == AuthorizationRight.Allow;
        }
    }
}