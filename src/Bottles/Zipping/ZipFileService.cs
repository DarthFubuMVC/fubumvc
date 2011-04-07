using System;
using System.IO;
using FubuCore;
using Ionic.Zip;
using System.Linq;

namespace Bottles.Zipping
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

        public void ExtractTo(string description, Stream stream, string folder)
        {
            Console.WriteLine("Writing contents of zip file {0} to {1}", description, folder);
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            Directory.CreateDirectory(folder);

            var system = new FileSystem();
            string fileName = Path.GetTempFileName();
            system.WriteStreamToFile(fileName, stream);

            using (var zipFile = new ZipFile(fileName))
            {
                zipFile.ExtractAll(folder, ExtractExistingFileAction.OverwriteSilently);
            }

            system.DeleteFile(fileName);
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

        public string GetVersion(string fileName)
        {
            using (var zipFile = new ZipFile(fileName))
            {

                var entry = zipFile.Entries.SingleOrDefault(x => x.FileName == FubuMvcPackages.VersionFile);
                if (entry == null) return Guid.Empty.ToString();

                var stream = new MemoryStream();
                entry.Extract(stream);

                stream.Position = 0;
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }
}