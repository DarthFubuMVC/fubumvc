using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security
{
    public class AuthorizedByAttributeConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Each(examineChain);
        }

        private static void examineChain(BehaviorChain chain)
        {
            chain.Calls.Each(c =>
            {
                c.ForAttributes<AuthorizedByAttribute>(att =>
                {
                    var inputType = c.InputType();
                    att.Types.Each(attType =>
                    {
                        var ruleType = RuleTypeFor(inputType, attType);
                        chain.Authorization.AddPolicy(ruleType);
                    });
                    
                    
                });
            });
        }

        public static Type RuleTypeFor(Type inputType, Type attributeType)
        {
            if (attributeType.CanBeCastTo<IAuthorizationPolicy>()) return attributeType;
        
            if (attributeType.Closes(typeof(IAuthorizationRule<>)))
            {
                return typeof (AuthorizationPolicy<,>).MakeGenericType(inputType, attributeType);
            }

            throw new ArgumentOutOfRangeException("attributeType", "attributeType must implement either IAuthorizationPolicy or IAuthorizationRule<> for the input type");
        }
    }
}