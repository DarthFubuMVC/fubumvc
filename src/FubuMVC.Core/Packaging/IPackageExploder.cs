using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageExploder
    {
        IEnumerable<string> ExplodeAllZipsAndReturnPackageDirectories(string applicationDirectory);
        void Explode(string applicationDirectory, string zipFile);
        void CleanAll(string applicationDirectory);
        string ReadVersion(string directoryName);
        void LogPackageState(string applicationDirectory);
    }
}