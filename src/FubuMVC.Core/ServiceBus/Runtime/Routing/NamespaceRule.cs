using System;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.Runtime.Routing
{
    public class NamespaceRule : IRoutingRule
    {
        private readonly string _ns;

        public NamespaceRule(string @namespace)
        {
            _ns = @namespace;
        }

        public bool Matches(Type type)
        {
            return type.IsInNamespace(_ns);
        }

        public string Describe()
        {
            return "Messages from namespace " + _ns;
        }

        public static NamespaceRule For<T>()
        {
            return new NamespaceRule(typeof(T).Namespace);
        }

        protected bool Equals(NamespaceRule other)
        {
            return string.Equals(_ns, other._ns);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NamespaceRule) obj);
        }

        public override int GetHashCode()
        {
            return (_ns != null ? _ns.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Contained in namespace {0}", _ns);
        }
    }
}