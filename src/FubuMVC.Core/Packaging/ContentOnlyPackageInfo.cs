using System;
using System.Collections.Generic;
using System.IO;
using Bottles;
using Bottles.Assemblies;

namespace FubuMVC.Core.Packaging
{
    public class ContentOnlyPackageInfo : IPackageInfo
    {
        private readonly string _directory;

        public ContentOnlyPackageInfo(string directory)
        {
            _directory = directory;
            Role = BottleRoles.Module;
        }

        public void LoadAssemblies(IAssemblyRegistration loader)
        {
            // Do nothing
        }

        public void ForFolder(string folderName, Action<string> onFound)
        {
            if (folderName == BottleFiles.WebContentFolder)
            {
                onFound(_directory);
            }
        }

        public void ForData(string searchPattern, Action<string, Stream> dataCallback)
        {
            // Do nothing
        }

        public IEnumerable<Dependency> GetDependencies()
        {
            // No dependencies
            yield break;
        }

        public string Name
        {
            get { return _directory; }
        }

        public string Role { get; set; }
    }
}