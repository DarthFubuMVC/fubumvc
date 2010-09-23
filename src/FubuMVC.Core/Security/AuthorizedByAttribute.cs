using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizedByAttribute : AuthorizationAttribute
    {
        private readonly Type[] _types;

        public AuthorizedByAttribute(params Type[] types)
        {
            _types = types;
        }

        public Type[] Types
        {
            get { return _types; }
        }

        public override void Alter(ActionCall call)
        {
            var inputType = call.InputType();
            Types.Each(attType =>
            {
                var ruleType = RuleTypeFor(inputType, attType);
                call.ParentChain().Authorization.AddPolicy(ruleType);
            });
        }

        public static Type RuleTypeFor(Type inputType, Type attributeType)
        {
            if (attributeType.CanBeCastTo<IAuthorizationPolicy>()) return attributeType;

            if (attributeType.Closes(typeof(IAuthorizationRule<>)))
            {
                return typeof(AuthorizationPolicy<,>).MakeGenericType(inputType, attributeType);
            }

            throw new ArgumentOutOfRangeException("attributeType", "attributeType must implement either IAuthorizationPolicy or IAuthorizationRule<> for the input type");
        }
    }

}