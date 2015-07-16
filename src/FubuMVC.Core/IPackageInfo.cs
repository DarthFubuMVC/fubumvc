using System;
using System.IO;

namespace Bottles
{
    public interface IPackageInfo
    {
        string Name { get; }
        string Role { get; }
        string Description { get; }

        void ForFolder(string folderName, Action<string> onFound);
        void ForFiles(string directory, string searchPattern, Action<string, Stream> fileCallback);

    }
}