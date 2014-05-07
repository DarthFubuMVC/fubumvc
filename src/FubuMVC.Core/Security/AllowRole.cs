using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using System.Collections.Generic;

namespace FubuMVC.Core.Security
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

        public override void Alter(ActionCall call)
        {
            var authorizationNode = call.ParentChain().Authorization;
            _roles.Each(r => authorizationNode.AddRole(r));
        }
    }

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
            return string.Format("Role: {0}", _role);
        }
    }
}