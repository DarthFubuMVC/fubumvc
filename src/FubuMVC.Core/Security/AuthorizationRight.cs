using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Security
{
    public class AuthorizationRight
    {
        private readonly int _precedence;

        private AuthorizationRight(int precedence)
        {
            _precedence = precedence;
        }

        public int Precedence
        {
            get { return _precedence; }
        }

        public static AuthorizationRight None = new AuthorizationRight(3);
        public static AuthorizationRight Allow = new AuthorizationRight(2);
        public static AuthorizationRight Deny = new AuthorizationRight(1);

        public static AuthorizationRight Combine(IEnumerable<AuthorizationRight> rights)
        {
            if (!rights.Any())
            {
                return AuthorizationRight.None;
            }

            return rights.OrderBy(x => x.Precedence).First(); 
        }

        public static AuthorizationRight CombineRights(params AuthorizationRight[] rights)
        {
            return Combine(rights);
        }
    }
}