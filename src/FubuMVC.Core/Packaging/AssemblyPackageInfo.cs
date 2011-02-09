using System;
using System.IO;
using System.Reflection;

namespace FubuMVC.Core.Packaging
{
    public class AssemblyPackageInfo : IPackageInfo
    {
        private readonly Assembly _assembly;
        private readonly PackageFiles _files = new PackageFiles();

        public AssemblyPackageInfo(Assembly assembly)
        {
            _assembly = assembly;
        }

        public PackageFiles Files
        {
            get { return _files; }
        }

        public string Name
        {
            get { return "Assembly:  " + _assembly.FullName; }
        }

        public void LoadAssemblies(IAssemblyRegistration loader)
        {
            loader.Use(_assembly);
        }

        public void ForFolder(string folderName, Action<string> onFound)
        {
            _files.ForFolder(folderName, onFound);
        }

        public void ForData(string searchPattern, Action<string, Stream> dataCallback)
        {
            _files.ForData(searchPattern, dataCallback);
        }
    }
}