using System;
using System.IO;

namespace Bottles.Zipping
{
    public interface IZipFileService
    {
        void CreateZipFile(string fileName, Action<IZipFile> configure);
        void ExtractTo(string fileName, string folder);
        string GetVersion(string fileName);
        void ExtractTo(string description, Stream stream, string folder);
    }
}