using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class AllowRoleAttribute : ModifyChainAttribute
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

        public override void Alter(ActionCallBase call)
        {
            var authorizationNode = call.ParentChain().Authorization;
            _roles.Each(r => authorizationNode.AddRole(r));
        }
    }

    /// <summary>
    /// Users with the designated role are allowed to access this resource
    /// </summary>
    public class AllowRole : IAuthorizationPolicy
    {
        private readonly string _role;

        public AllowRole(string role)
        {
            _role = role;
        }

        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            return PrincipalRoles.IsInRole(_role) ? AuthorizationRight.Allow : AuthorizationRight.None;
        }

        public string Role
        {
            get { return _role; }
        }

        public override string ToString()
        {
            return string.Format("Allow for role: {0}", _role);
        }
    }
}