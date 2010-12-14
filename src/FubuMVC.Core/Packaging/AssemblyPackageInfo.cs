using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

namespace FubuMVC.Core.Packaging
{
    public class AssemblyPackageInfo : IPackageInfo
    {
        private readonly Assembly _assembly;

        public AssemblyPackageInfo(Assembly assembly)
        {
            _assembly = assembly;
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
            // do nothing
        }

        public void ForData(string searchPattern, Action<string, Stream> dataCallback)
        {
            // do nothing
        }
    }
}