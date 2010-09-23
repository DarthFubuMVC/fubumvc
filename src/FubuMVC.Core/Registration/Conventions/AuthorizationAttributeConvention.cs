using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Registration.Conventions
{
    public class AuthorizationAttributeConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Each(analyzeChain);
        }

        private static void analyzeChain(BehaviorChain chain)
        {
            chain.Calls.Each(call => call.ForAttributes<AuthorizationAttribute>(att => att.Alter(call)));
        }
    }


}