using System;
using System.IO;
using Ionic.Zip;
using System.Linq;

namespace FubuMVC.Core.Packaging
{
    public class ZipFileService : IZipFileService
    {
        public void CreateZipFile(string fileName, Action<IZipFile> configure)
        {
            Console.WriteLine("Starting to write contents to new Zip file at " + fileName);
            using (var zipFile = new ZipFile(fileName))
            {
                configure(new ZipFileWrapper(zipFile));
                zipFile.Save();
            }
        }

        public void ExtractTo(string fileName, string folder)
        {
            Console.WriteLine("Writing contents of zip file {0} to {1}", fileName, folder);
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            Directory.CreateDirectory(folder);

            using (var zipFile = new ZipFile(fileName))
            {
                zipFile.ExtractAll(folder, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public Guid GetVersion(string fileName)
        {
            using (var zipFile = new ZipFile(fileName))
            {

                var entry = zipFile.Entries.SingleOrDefault(x => x.FileName == FubuMvcPackages.VersionFile);
                if (entry == null) return Guid.Empty;

                var stream = new MemoryStream();
                entry.Extract(stream);

                stream.Position = 0;
                var reader = new StreamReader(stream);
                string raw = reader.ReadToEnd();

                return new Guid(raw);
            }
        }
    }
}