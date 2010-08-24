using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Core.Security
{
    public interface IAuthorizationPreviewService
    {
        bool IsAuthorized(object model);
        bool IsAuthorized(object model, string category);
        bool IsAuthorized<TController>(Expression<Action<TController>> expression);

        bool IsAuthorizedForNew<T>();
        bool IsAuthorizedForNew(Type entityType);

        bool IsAuthorizedForPropertyUpdate(object model);
        bool IsAuthorizedForPropertyUpdate(Type type);

        bool IsAuthorizedFor(Type handlerType, MethodInfo method);
    }

    public class AuthorizationPreviewService : ChainInterrogator<AuthorizationRight>, IAuthorizationPreviewService
    {
        private readonly IEndPointAuthorizorFactory _factory;

        public AuthorizationPreviewService(IChainResolver resolver, IEndPointAuthorizorFactory factory) : base(resolver)
        {
            _factory = factory;
        }

        protected override AuthorizationRight applyForwarder(object model, IChainForwarder forwarder)
        {
            throw new NotImplementedException();
        }

        protected override AuthorizationRight findAnswerFromResolver(object model, Func<IChainResolver, BehaviorChain> finder)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized(object model)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized(object model, string category)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized<TController>(Expression<Action<TController>> expression)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorizedForNew<T>()
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorizedForNew(Type entityType)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorizedForPropertyUpdate(object model)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorizedForPropertyUpdate(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorizedFor(Type handlerType, MethodInfo method)
        {
            throw new NotImplementedException();
        }
    }

    public interface IEndPointAuthorizor
    {
        AuthorizationRight IsAuthorized(IFubuRequest request);
    }

    public class EndPointAuthorizor : IEndPointAuthorizor
    {
        private readonly IEnumerable<IAuthorizationPolicy> _policies;

        public EndPointAuthorizor(IEnumerable<IAuthorizationPolicy> policies)
        {
            if (!policies.Any())
            {
                throw new ArgumentOutOfRangeException("policies", "At least one authorization policy is required");    
            }

            _policies = policies;
        }

        public AuthorizationRight IsAuthorized(IFubuRequest request)
        {
            return AuthorizationRight.Combine(_policies.Select(x => x.RightsFor(request)));
        }
    }

    public interface IEndPointAuthorizorFactory
    {
        IEndPointAuthorizor AuthorizorFor(Guid behaviorId);
    }

    // TODO -- need to register this one
    public class EndPointAuthorizorFactory : IEndPointAuthorizorFactory
    {
        public IEndPointAuthorizor AuthorizorFor(Guid behaviorId)
        {
            throw new NotImplementedException();
        }
    }
}