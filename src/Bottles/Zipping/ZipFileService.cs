using System;
using System.IO;
using Bottles.Exploding;
using FubuCore;
using Ionic.Zip;
using System.Linq;

namespace Bottles.Zipping
{
    public class ZipFileService : IZipFileService
    {
        private readonly IFileSystem _fileSystem;

        public ZipFileService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void CreateZipFile(string fileName, Action<IZipFile> configure)
        {
            Console.WriteLine("Starting to write contents to new Zip file at " + fileName);
            using (var zipFile = new ZipFile(fileName))
            {
                configure(new ZipFileWrapper(zipFile));
                zipFile.Save();
            }
        }

        public void ExtractTo(string description, Stream stream, string directory)
        {
            Console.WriteLine("Writing contents of zip file {0} to {1}", description, directory);

            _fileSystem.DeleteDirectory(directory);
            _fileSystem.CreateDirectory(directory);

            
            string fileName = Path.GetTempFileName();
            _fileSystem.WriteStreamToFile(fileName, stream);

            using (var zipFile = new ZipFile(fileName))
            {
                zipFile.ExtractAll(directory, ExtractExistingFileAction.OverwriteSilently);
            }

            _fileSystem.DeleteFile(fileName);
        }

        public void ExtractTo(string fileName, string directory, ExplodeOptions options)
        {
            Console.WriteLine("Writing contents of zip file {0} to {1}", fileName, directory);

            if (options == ExplodeOptions.DeleteDestination)
            {
                _fileSystem.DeleteDirectory(directory);
            }

            _fileSystem.CreateDirectory(directory);

            using (var zipFile = new ZipFile(fileName))
            {
                zipFile.ExtractAll(directory, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public string GetVersion(string fileName)
        {
            using (var zipFile = new ZipFile(fileName))
            {

                var entry = zipFile.Entries.SingleOrDefault(x => x.FileName == BottleFiles.VersionFile);
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