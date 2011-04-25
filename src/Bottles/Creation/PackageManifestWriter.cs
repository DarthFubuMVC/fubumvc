using System;
using FubuCore;

namespace Bottles.Creation
{
    public class PackageManifestWriter<T> where T : PackageManifest, new()
    {
        private readonly IFileSystem _fileSystem;
        private T _manifest;

        public PackageManifestWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void ReadFrom(string fileName, Action<T> onCreation)
        {
            if (_fileSystem.FileExists(fileName))
            {
                _manifest = _fileSystem.LoadFromFile<T>(fileName);
            }
            else
            {
                _manifest = new T();
                onCreation(_manifest);
            }
        }

        public T Manifest
        {
            get { return _manifest; }
        }

        public void WriteTo(string fileName)
        {
            _fileSystem.WriteObjectToFile(fileName, _manifest);
        }

        public void AddAssembly(string assemblyName)
        {
            Manifest.AddAssembly(assemblyName);
        }
    }
}