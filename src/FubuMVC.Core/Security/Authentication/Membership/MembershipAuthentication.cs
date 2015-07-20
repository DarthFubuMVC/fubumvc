using System;
using System.Runtime.Serialization;
using System.Security.Principal;
using FubuCore;

namespace FubuMVC.Core.Security.Authentication.Membership
{
    public class MembershipAuthentication : IAuthenticationStrategy, IPrincipalBuilder, ICredentialsAuthenticator
    {
        private readonly IMembershipRepository _membership;
        private readonly BasicAuthentication _inner;

        public MembershipAuthentication(IAuthenticationSession session, IPrincipalContext context, IMembershipRepository membership, ILockedOutRule lockedOutRule)
        {
            _membership = membership;
            _inner = new BasicAuthentication(session, context, this, this, lockedOutRule);
        }

        public AuthResult TryToApply()
        {
            return _inner.TryToApply();
        }

        public bool Authenticate(LoginRequest request)
        {
            return _inner.Authenticate(request);
        }

        public bool AuthenticateCredentials(LoginRequest request)
        {
            if (request.UserName.IsEmpty()) return false;

            return _membership.MatchesCredentials(request);
        }

        public IMembershipRepository Membership
        {
            get { return _membership; }
        }

        public IPrincipal Build(string userName)
        {
            var user = _membership.FindByName(userName);
            if (user == null)
            {
                throw new UnknownUserException(userName);
            }

            return new FubuPrincipal(user);
        }
    }

    [Serializable]
    public class UnknownUserException : Exception
    {
        public UnknownUserException(string user) : base("User {0} cannot be found".ToFormat(user))
        {
        }

        protected UnknownUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}