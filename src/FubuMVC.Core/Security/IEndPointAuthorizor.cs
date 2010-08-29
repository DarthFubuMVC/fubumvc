using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    public interface IEndPointAuthorizor
    {
        AuthorizationRight IsAuthorized(IFubuRequest request);
    }

    public class NulloEndPointAuthorizor : IEndPointAuthorizor
    {
        public static NulloEndPointAuthorizor Flyweight = new NulloEndPointAuthorizor();

        public AuthorizationRight IsAuthorized(IFubuRequest request)
        {
            return AuthorizationRight.Allow;
        }
    }
}