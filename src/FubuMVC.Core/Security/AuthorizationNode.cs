using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using System.Linq;

namespace FubuMVC.Core.Security
{

    // TODO -- add ability to specify the authorization failure handling
    public interface IAuthorizationNode
    {
        /// <summary>
        /// Adds an authorization rule based on membership in a given role
        /// on the principal
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        AllowRole AddRole(string roleName);

        /// <summary>
        /// List of all roles that have privileges to this BehaviorChain endpoint
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> AllowedRoles();

        /// <summary>
        /// Simple boolean test of whether or not this BehaviorChain has any
        /// authorization rules
        /// </summary>
        /// <returns></returns>
        bool HasRules();


        /// <summary>
        /// Adds an authorization policy to this behavior chain
        /// </summary>
        /// <param name="policy"></param>
        void AddPolicy(IAuthorizationPolicy policy);


        AuthorizationRight IsAuthorized(IFubuRequestContext context);
        IEnumerable<IAuthorizationPolicy> Policies { get; }
    }

    [Description("Authorization checks for this endpoint")]
    public class AuthorizationNode : BehaviorNode, IAuthorizationNode
    {
        private readonly IList<IAuthorizationPolicy> _policies = new List<IAuthorizationPolicy>();

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Authorization; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var def = ObjectDef.ForType<AuthorizationBehavior>();
            def.DependencyByValue<IAuthorizationNode>(this);

            // TODO -- add the failure handler too

            return def;
        }

        public AuthorizationRight IsAuthorized(IFubuRequestContext context)
        {
            if (!_policies.Any()) return AuthorizationRight.Allow;

            return AuthorizationRight.Combine(_policies.Select(x => x.RightsFor(context)));
        }

        public IEnumerable<IAuthorizationPolicy> Policies
        {
            get { return _policies; }
        }

        /// <summary>
        /// Adds an authorization rule based on membership in a given role
        /// on the principal
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public AllowRole AddRole(string roleName)
        {
            if (AllowedRoles().Contains(roleName)) return null;

            var allow = new AllowRole(roleName);

            _policies.Add(allow);

            return allow;
        }

        /// <summary>
        /// Adds an authorization policy to this behavior chain
        /// </summary>
        /// <param name="policy"></param>
        public void AddPolicy(IAuthorizationPolicy policy)
        {
            _policies.Add(policy);
        }

        /// <summary>
        /// List of all roles that have privileges to this BehaviorChain endpoint
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> AllowedRoles()
        {
            return _policies.OfType<AllowRole>().Select(x => x.Role);
        }

        /// <summary>
        /// Simple boolean test of whether or not this BehaviorChain has any
        /// authorization rules
        /// </summary>
        /// <returns></returns>
        public bool HasRules()
        {
            return _policies.Any();
        }
    }
}