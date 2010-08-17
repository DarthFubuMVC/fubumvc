using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    public class AuthorizationRight
    {
        private readonly int _precedence;

        private AuthorizationRight(int precedence)
        {
            _precedence = precedence;
        }

        public int Precedence
        {
            get { return _precedence; }
        }

        public static AuthorizationRight None = new AuthorizationRight(3);
        public static AuthorizationRight Allow = new AuthorizationRight(2);
        public static AuthorizationRight Deny = new AuthorizationRight(1);

        public static AuthorizationRight Combine(IEnumerable<AuthorizationRight> rights)
        {
            if (!rights.Any())
            {
                return AuthorizationRight.None;
            }

            return rights.OrderBy(x => x.Precedence).First(); 
        }

        public static AuthorizationRight CombineRights(params AuthorizationRight[] rights)
        {
            return Combine(rights);
        }
    }

    public interface IAuthorizationPolicy
    {
        AuthorizationRight RightsFor(IFubuRequest request); 
    }

    public interface IAuthorizationFailureHandler
    {
        void Handle();
    }

    public class DefaultAuthorizationFailureHandler : IAuthorizationFailureHandler
    {
        private readonly IOutputWriter _writer;

        public DefaultAuthorizationFailureHandler(IOutputWriter writer)
        {
            _writer = writer;
        }

        public void Handle()
        {
            _writer.WriteResponseCode(HttpStatusCode.Forbidden);
        }
    }


    /******************************************EXPERIMENTAL*****************************************
    public class AuthorizationPolicy<TRule, TModel> : IAuthorizationPolicy where TRule : IAuthorizationRule<TModel> where TModel : class
    {
        private readonly TRule _innerRule;

        public AuthorizationPolicy(TRule innerRule)
        {
            _innerRule = innerRule;
        }

        public AuthorizationRight RightsFor(IFubuRequest request)
        {
            return _innerRule.RightsFor(request.Get<TModel>());
        }
    }

    public interface IAuthorizationRule<in T>
    {
        AuthorizationRight RightsFor(T model);
    }
    **********************************************************************************************/


    public class AuthorizationBehavior : BasicBehavior
    {
        private readonly IAuthorizationFailureHandler _failureHandler;
        private readonly IFubuRequest _request;
        private readonly IEnumerable<IAuthorizationPolicy> _policies;

        public AuthorizationBehavior(IAuthorizationFailureHandler failureHandler, IFubuRequest request, IEnumerable<IAuthorizationPolicy> policies) : base(PartialBehavior.Executes)
        {
            _failureHandler = failureHandler;
            _request = request;
            _policies = policies;
        }

        protected override DoNext performInvoke()
        {
            var rights = _policies.Select(x => x.RightsFor(_request));
            var access = AuthorizationRight.Combine(rights);

            if (access == AuthorizationRight.Allow)
            {
                return DoNext.Continue;
            }


            _failureHandler.Handle();

            return DoNext.Stop;
        }
    }
}