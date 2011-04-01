using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using System.Linq;

namespace FubuMVC.Core.Security
{
    public interface IAuthorizationRegistration
    {
        ObjectDef ToEndpointAuthorizorObjectDef();
        void Register(Guid uniqueId, Action<Type, ObjectDef> callback);
    }

    public class AuthorizationNode : BehaviorNode, IAuthorizationRegistration
    {
        private readonly ListDependency _policies = new ListDependency(typeof(IEnumerable<IAuthorizationPolicy>));

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Authorization; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var objectDef = new ObjectDef(typeof(AuthorizationBehavior));
            objectDef.Dependencies.Add(_policies);

            return objectDef;
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

            _policies.AddValue(allow);


            return allow;
        }

        /// <summary>
        /// Adds an authorization rule of type IAuthorizationRule<TModel>
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TRule"></typeparam>
        /// <returns></returns>
        public ObjectDef AddPolicy<TModel, TRule>() where TRule : IAuthorizationRule<TModel> where TModel : class
        {
            var topDef = _policies.AddType(typeof (AuthorizationPolicy<TModel>));

            var ruleObjectDef = new ObjectDef(typeof (TRule));
            topDef.Dependencies.Add(new ConfiguredDependency(){
                DependencyType = typeof (IAuthorizationRule<TModel>),
                Definition = ruleObjectDef
            });

            return ruleObjectDef;
        }

        /// <summary>
        /// Adds an authorization policy of the given policyType to this behavior chain
        /// </summary>
        /// <param name="policyType"></param>
        /// <returns></returns>
        public ObjectDef AddPolicy(Type policyType)
        {
            // TODO -- blow up if this isn't IAuthorizationPolicy
            return _policies.AddType(policyType);
        }

        /// <summary>
        /// Adds an authorization policy to this behavior chain
        /// </summary>
        /// <param name="policy"></param>
        public void AddPolicy(IAuthorizationPolicy policy)
        {
            _policies.AddValue(policy);
        }

        /// <summary>
        /// List of all roles that have privileges to this BehaviorChain endpoint
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> AllowedRoles()
        {
            return _policies.Items.Where(x => x.Value is AllowRole).Select(x => x.Value.As<AllowRole>().Role);
        }

        /// <summary>
        /// Simple boolean test of whether or not this BehaviorChain has any
        /// authorization rules
        /// </summary>
        /// <returns></returns>
        public bool HasRules()
        {
            return _policies.Items.Any();
        }

        ObjectDef IAuthorizationRegistration.ToEndpointAuthorizorObjectDef()
        {
            return toEndpointAuthorizationObjectDef();
        }

        private ObjectDef toEndpointAuthorizationObjectDef()
        {
            var objectDef = new ObjectDef(typeof(EndPointAuthorizor)){
                Name = ParentChain().UniqueId.ToString()
            };

            objectDef.Dependencies.Add(_policies);

            return objectDef;
        }

        void IAuthorizationRegistration.Register(Guid uniqueId, Action<Type, ObjectDef> callback)
        {
            var objectDef = HasRules()
                ? toEndpointAuthorizationObjectDef()
                : new ObjectDef(){
                    Value = NulloEndPointAuthorizor.Flyweight
                };

            objectDef.Name = uniqueId.ToString();

            callback(typeof (IEndPointAuthorizor), objectDef);

        }

        public IEnumerable<ObjectDef> AllRules
        {
            get
            {
                return _policies.Items;
            }
        }

        public override string ToString()
        {
            return base.ToString() + Environment.NewLine + AllRules.Select(x => x.Value != null ?  x.Value.ToString() : x.Type.ToString()).Join(Environment.NewLine);
        }
    }
}