using System;
using System.Reflection;

namespace FubuMVC.Core.ServiceBus.Runtime.Routing
{
    public class AssemblyRule : IRoutingRule
    {
        private readonly Assembly _assembly;

        public AssemblyRule(Assembly assembly)
        {
            _assembly = assembly;
        }

        public bool Matches(Type type)
        {
            return _assembly.Equals(type.Assembly);
        }

        public string Describe()
        {
            return "Messages in Assembly " + _assembly.GetName().Name;
        }

        public static AssemblyRule For<T>()
        {
            return new AssemblyRule(typeof(T).Assembly);
        }

        protected bool Equals(AssemblyRule other)
        {
            return Equals(_assembly, other._assembly);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssemblyRule) obj);
        }

        public override int GetHashCode()
        {
            return (_assembly != null ? _assembly.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Contained in assembly {0}", _assembly.GetName().Name);
        }
    }
}