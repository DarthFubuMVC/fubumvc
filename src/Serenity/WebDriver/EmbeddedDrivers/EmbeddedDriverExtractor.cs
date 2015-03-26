using System;
using System.IO;
using FubuCore;

namespace Serenity.WebDriver.EmbeddedDrivers
{
    public class EmbeddedDriverExtractor<TEmbeddedDriver> : IEmbeddedDriverExtractor<TEmbeddedDriver> where TEmbeddedDriver : IEmbeddedDriver, new()
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEmbeddedDriver _embeddedDriver;

        public string PathToVersionDeclarationFile { get; private set; }

        public string PathToDriver { get; private set; }

        public EmbeddedDriverExtractor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _embeddedDriver = new TEmbeddedDriver();

            PathToDriver = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _embeddedDriver.ExtractedFileName);
            PathToVersionDeclarationFile = PathToDriver + ".version";
        }

        public bool ShouldExtract()
        {
            if (!_fileSystem.FileExists(PathToVersionDeclarationFile) || !_fileSystem.FileExists(PathToDriver))
                return true;

            var currentVersion = new Version(_fileSystem.ReadStringFromFile(PathToVersionDeclarationFile));

            return currentVersion < _embeddedDriver.Version;
        }

        public void Extract()
        {
            _fileSystem.DeleteFile(PathToDriver);
            _fileSystem.DeleteFile(PathToVersionDeclarationFile);

            var type = GetType();

            using (var driverStream = type.Assembly.GetManifestResourceStream(type, _embeddedDriver.ResourceName))
            {
                _fileSystem.WriteStreamToFile(PathToDriver, driverStream);
                _fileSystem.WriteStringToFile(PathToVersionDeclarationFile, _embeddedDriver.Version.ToString());
            }
        }
    }
}