using System;

namespace FubuMVC.Core.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizedByAttribute : Attribute
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
    }

}