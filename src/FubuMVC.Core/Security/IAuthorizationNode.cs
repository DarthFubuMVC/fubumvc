using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Security
{
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
        void AddPolicies(IEnumerable<IAuthorizationPolicy> authorizationPolicies);
        void FailureHandler<T>() where T : IAuthorizationFailureHandler;
        void FailureHandler(IAuthorizationFailureHandler handler);
        ObjectDef FailureHandler();
        void FailureHandler(Type handlerType);

        /// <summary>
        /// Add either an IAuthorizationPolicy or IAuthorizationCheck to 
        /// this chain by concrete type
        /// </summary>
        /// <param name="type"></param>
        void Add(Type type);
    }
}