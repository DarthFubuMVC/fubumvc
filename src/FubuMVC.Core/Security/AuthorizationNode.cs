using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Security
{
    public class AuthorizationNode : BehaviorNode
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

        public AllowRole AddRole(string roleName)
        {
            var allow = new AllowRole(roleName);

            _policies.AddValue(allow);


            return allow;
        }
    }
}