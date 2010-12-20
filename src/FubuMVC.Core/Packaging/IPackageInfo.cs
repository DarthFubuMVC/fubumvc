using System;
using System.IO;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageInfo
    {
        string Name { get; }
        void LoadAssemblies(IAssemblyRegistration loader);

        void ForFolder(string folderName, Action<string> onFound);
        void ForData(string searchPattern, Action<string, Stream> dataCallback);
    }
}