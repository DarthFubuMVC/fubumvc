using System;

namespace Fubu.Packages
{
    public interface IZipFileService
    {
        void CreateZipFile(string fileName, Action<IZipFile> configure);
        void ExtractTo(string fileName, string folder);
        Guid GetVersion(string fileName);
    }
}