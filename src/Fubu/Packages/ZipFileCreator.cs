using System;
using Ionic.Zip;

namespace Fubu.Packages
{
    public class ZipFileCreator : IZipFileCreator
    {
        public void CreateZipFile(string fileName, Action<IZipFile> configure)
        {
            using (var zipFile = new ZipFile(fileName))
            {
                configure(new ZipFileWrapper(zipFile));
                zipFile.Save();
            }
        }
    }
}