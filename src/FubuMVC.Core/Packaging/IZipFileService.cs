using System;

namespace FubuMVC.Core.Packaging
{
    public interface IZipFileService
    {
        void CreateZipFile(string fileName, Action<IZipFile> configure);
        void ExtractTo(string fileName, string folder);
        string GetVersion(string fileName);
    }
}