using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Security
{
    public class AuthorizationRight
    {
        private readonly int _precedence;
        private readonly string _name;

        private AuthorizationRight(int precedence, string name)
        {
            _precedence = precedence;
            _name = name;
        }

        public int Precedence
        {
            get { return _precedence; }
        }

        public string Name
        {
            get { return _name; }
        }

        public static AuthorizationRight None = new AuthorizationRight(3, "None");
        public static AuthorizationRight Allow = new AuthorizationRight(2, "Allow");
        public static AuthorizationRight Deny = new AuthorizationRight(1, "Deny");

        public static AuthorizationRight Combine(IEnumerable<AuthorizationRight> rights)
        {
            var authorizationRights = rights.ToArray();

            if (!authorizationRights.Any())
            {
                return AuthorizationRight.None;
            }

            return authorizationRights.OrderBy(x => x.Precedence).First(); 
        }

        public static AuthorizationRight CombineRights(params AuthorizationRight[] rights)
        {
            return Combine(rights);
        }

        public override string ToString()
        {
            return string.Format("AuthorizationRight: {0}", _name);
        }
    }
}