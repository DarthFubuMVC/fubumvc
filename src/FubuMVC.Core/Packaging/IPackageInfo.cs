using System;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageInfo
    {
        string Name { get; }
        void LoadAssemblies(IAssemblyRegistration loader);

        void ForFolder(string folderName, Action<string> onFound);
    }
}