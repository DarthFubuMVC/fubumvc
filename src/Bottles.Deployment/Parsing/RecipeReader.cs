using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace Bottles.Deployment.Parsing
{
    public class RecipeReader
    {
        private readonly string _directory;
        private readonly IFileSystem _fileSystem = new FileSystem();

        public RecipeReader(string directory)
        {
            _directory = directory;
        }

        public Recipe Read()
        {
            var recipe = new Recipe();

            // need to read the recipe control file
            // need to read each host file

            
            _fileSystem.FindFiles(_directory, new FileSet(){
                Include = "*.host"
            }).Each(file =>
            {
                throw new NotImplementedException();
            });
            
            throw new NotImplementedException();
        }
    }

    public class HostReader
    {
        public HostManifest ReadFrom(string fileName)
        {
            var parser = new SettingsParser(fileName);
            new FileSystem().ReadTextFile(fileName, parser.ParseText);

            var hostName = Path.GetFileNameWithoutExtension(fileName);
            var host = new HostManifest(hostName);

            host.RegisterSettings(parser.Settings);
            host.RegisterBottles(parser.References);

            return host;
        }
    }
}