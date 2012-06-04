using System.Reflection;

namespace FubuMVC.IntegrationTesting.Querying
{
    public class AssemblyToken
    {
        public AssemblyToken()
        {
        }

        public AssemblyToken(Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            Name = assemblyName.Name;
            Version = assemblyName.Version.ToString();
            FullName = assemblyName.FullName;
        }

        public string Name { get; set;}
        public string Version { get; set;}
        public string FullName { get; set;}

        public bool Equals(AssemblyToken other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Version, Version) && Equals(other.FullName, FullName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AssemblyToken)) return false;
            return Equals((AssemblyToken) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (Version != null ? Version.GetHashCode() : 0);
                result = (result*397) ^ (FullName != null ? FullName.GetHashCode() : 0);
                return result;
            }
        }
    }
}