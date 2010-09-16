using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AllowRoleAttribute : Attribute
    {
        private readonly string[] _roles;

        public AllowRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public string[] Roles
        {
            get { return _roles; }
        }
    }

    public class AllowRole : IAuthorizationPolicy
    {
        private readonly string _role;

        public AllowRole(string role)
        {
            _role = role;
        }

        public AuthorizationRight RightsFor(IFubuRequest request)
        {
            return PrincipalRoles.IsInRole(_role) ? AuthorizationRight.Allow : AuthorizationRight.None;
        }

        public string Role
        {
            get { return _role; }
        }
    }
}