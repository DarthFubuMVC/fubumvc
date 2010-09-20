using System;
using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    public interface IEndPointAuthorizor
    {
        AuthorizationRight IsAuthorized(IFubuRequest request);
        IEnumerable<string> RulesDescriptions();
    }

    public class NulloEndPointAuthorizor : IEndPointAuthorizor
    {
        public static NulloEndPointAuthorizor Flyweight = new NulloEndPointAuthorizor();

        public AuthorizationRight IsAuthorized(IFubuRequest request)
        {
            return AuthorizationRight.Allow;
        }

        public IEnumerable<string> RulesDescriptions()
        {
            yield return "None";
        }
    }
}