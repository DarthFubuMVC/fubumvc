using System;
using System.Collections.Generic;
using FubuCore;

namespace Bottles.Deployment.Parsing
{
    public class ProfileReader
    {
        private readonly IFileSystem _fileSystem = new FileSystem();

        public IEnumerable<HostManifest> ReadHosts(string directory)
        {
            throw new NotImplementedException();
            //Directory.GetDirectories(ProfileFiles.RecipesFolder).Each()   
        }
    }
}