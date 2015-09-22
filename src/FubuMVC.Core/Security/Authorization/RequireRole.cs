using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authorization
{
    public class RequireRoleAttribute : ModifyChainAttribute
    {
        private readonly string[] _roles;

        public RequireRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void Alter(ActionCallBase call)
        {
            var chain = call.ParentChain();
            _roles.Each(role => chain.Authorization.AddPolicy(new RequireRole(role)));
        }
    }

    /// <summary>
    /// Enforces that a user *must* have the designated role to have access
    /// to this resource
    /// </summary>
    public class RequireRole : IAuthorizationPolicy
    {
        private readonly string _role;

        public RequireRole(string role)
        {
            _role = role;
        }

        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            return PrincipalRoles.IsInRole(_role) ? AuthorizationRight.Allow : AuthorizationRight.Deny;
        }

        public string Role
        {
            get { return _role; }
        }

        public override string ToString()
        {
            return string.Format("Require role: {0}", _role);
        }

        protected bool Equals(RequireRole other)
        {
            return string.Equals(_role, other._role);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RequireRole) obj);
        }

        public override int GetHashCode()
        {
            return (_role != null ? _role.GetHashCode() : 0);
        }
    }
}