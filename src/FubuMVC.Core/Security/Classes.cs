using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public static AuthorizationRight None = new AuthorizationRight(3);
        public static AuthorizationRight Allow = new AuthorizationRight(2);
        public static AuthorizationRight Deny = new AuthorizationRight(1);

        public static AuthorizationRight Combine(IEnumerable<AuthorizationRight> rights)
        {
            return rights.Any() 
                ? rights.OrderBy(x => x._precedence).First() 
                : AuthorizationRight.None;
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

}