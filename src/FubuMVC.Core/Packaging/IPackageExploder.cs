using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageExploder
    {
        void ExplodeAll(string applicationDirectory);
        void Explode(string applicationDirectory, string zipFile);
        void CleanAll(string applicationDirectory);
        Guid ReadVersion(string directoryName);
        void LogPackageState(string applicationDirectory);
        IEnumerable<string> FindExplodedPackageDirectories(string applicationDirectory);
    }
}