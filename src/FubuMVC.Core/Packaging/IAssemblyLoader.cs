using System.Reflection;

namespace FubuMVC.Core.Packaging
{
    public interface IAssemblyRegistration
    {
        void Use(Assembly assembly);
        void LoadFromFile(string fileName, string assemblyName);
    }

    public interface IAssemblyLoader
    {
        void ReadPackage(IPackageInfo package);
    }
}