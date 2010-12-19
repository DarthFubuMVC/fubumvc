using System;

namespace Fubu.Packages
{
    public interface IZipFileCreator
    {
        void CreateZipFile(string fileName, Action<IZipFile> configure);
    }
}