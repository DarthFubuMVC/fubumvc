using System;

namespace FubuMVC.IntegrationTesting.Querying
{
    public class TypeToken
    {
        public TypeToken()
        {
        }

        public TypeToken(Type type)
        {
            Name = type.Name;
            Namespace = type.Namespace;
            Assembly = new AssemblyToken(type.Assembly);
        }

        public string Name { get; set; }
        public string Namespace { get; set; }
        public AssemblyToken Assembly { get; set; }

        public bool Equals(TypeToken other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Namespace, Namespace) && Equals(other.Assembly, Assembly);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TypeToken)) return false;
            return Equals((TypeToken) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
                result = (result*397) ^ (Assembly != null ? Assembly.GetHashCode() : 0);
                return result;
            }
        }
    }
}