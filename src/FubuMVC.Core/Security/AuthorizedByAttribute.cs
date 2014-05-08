using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security
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

        public override void Alter(ActionCall call)
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

        public override void Alter(ActionCall call)
        {
            var authorizationNode = call.ParentChain().Authorization;

            Types.Each(attType =>
            {
                if (attType.CanBeCastTo<IAuthorizationPolicy>() && attType.IsConcreteWithDefaultCtor())
                {
                    var policy = Activator.CreateInstance(attType).As<IAuthorizationPolicy>();
                    authorizationNode.AddPolicy(policy);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Type {0} is not a concrete type of {1} with a default ctor".ToFormat(attType.FullName, typeof(IAuthorizationPolicy).FullName));
                }
            });
        }

        public static Type RuleTypeFor(Type inputType, Type attributeType)
        {
            if (attributeType.CanBeCastTo<IAuthorizationPolicy>()) return attributeType;

            throw new ArgumentOutOfRangeException("attributeType", "attributeType must implement either IAuthorizationPolicy or IAuthorizationRule<> for the input type");
        }
    }

}