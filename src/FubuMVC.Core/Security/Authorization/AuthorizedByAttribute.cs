using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizationFailureAttribute : ModifyChainAttribute
    {
        private Type _type;

        public AuthorizationFailureAttribute(Type failureHandler)
        {
            if (!failureHandler.CanBeCastTo<IAuthorizationFailureHandler>())
            {
                throw new ArgumentOutOfRangeException("failureHandler", "Must be a concrete type of IAuthorizationFailureHandler");
            }

            _type = failureHandler;
        }

        public override void Alter(ActionCallBase call)
        {
            call.ParentChain().Authorization.FailureHandler(_type);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizedByAttribute : ModifyChainAttribute
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

        public override void Alter(ActionCallBase call)
        {
            var authorizationNode = call.ParentChain().Authorization;

            Types.Each(authorizationNode.Add);
        }

        public static Type RuleTypeFor(Type inputType, Type attributeType)
        {
            if (attributeType.CanBeCastTo<IAuthorizationPolicy>()) return attributeType;

            throw new ArgumentOutOfRangeException("attributeType", "attributeType must implement either IAuthorizationPolicy or IAuthorizationRule<> for the input type");
        }
    }

}