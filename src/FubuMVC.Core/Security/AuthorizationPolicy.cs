using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    public class AuthorizationPolicy<TModel> : IAuthorizationPolicy where TModel : class
    {
        private readonly IAuthorizationRule<TModel> _innerRule;

        public AuthorizationPolicy(IAuthorizationRule<TModel> innerRule)
        {
            _innerRule = innerRule;
        }

        public AuthorizationRight RightsFor(IFubuRequest request)
        {
            return _innerRule.RightsFor(request.Get<TModel>());
        }

        public IAuthorizationRule<TModel> InnerRule
        {
            get { return _innerRule; }
        }

        public override string ToString()
        {
            return _innerRule.ToString();
        }
    }

    public class AuthorizationPolicy<TModel, TRule> : AuthorizationPolicy<TModel> where TRule : IAuthorizationRule<TModel> where TModel : class
    {
        public AuthorizationPolicy(TRule innerRule) : base(innerRule)
        {
        }
    }
}