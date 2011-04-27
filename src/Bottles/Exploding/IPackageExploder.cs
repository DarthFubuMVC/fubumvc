using System.Collections.Generic;

namespace Bottles.Exploding
{
    public interface IPackageExploder
    {
        IEnumerable<string> ExplodeAllZipsAndReturnPackageDirectories(string applicationDirectory);
        void Explode(string sourceZipFile, string destinationDirectory, ExplodeOptions options);
        void CleanAll(string applicationDirectory);
        string ReadVersion(string directoryName);
        void LogPackageState(string applicationDirectory);
    }
}