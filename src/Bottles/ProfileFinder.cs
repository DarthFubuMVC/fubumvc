using System;
using System.Collections.Generic;
using System.Text;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles
{
    public interface IProfileFinder
    {
        string FindDeploymentFolder(string startPath);
    }

    public class ProfileFinder : IProfileFinder
    {
        private readonly IFileSystem _fileSystem;
        private readonly IList<string> _pathsChecked;

        public ProfileFinder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _pathsChecked = new List<string>();
        }

        public string FindDeploymentFolder(string startPath)
        {
            var path = FileSystem.Combine(startPath, ProfileFiles.BottlesManifestFile);
            if (_fileSystem.FileExists(path))
            {
                ConsoleWriter.Write("Found deployment folder at {0}", path);
                return _fileSystem.GetDirectory(path);
            }
                

            _pathsChecked.Add(path);


            path = FileSystem.Combine(startPath, ProfileFiles.DeploymentFolder, ProfileFiles.BottlesManifestFile);
            if (_fileSystem.FileExists(path))
            {
                ConsoleWriter.Write("Found deployment folder at {0}", path);
                return _fileSystem.GetDirectory(path);
            }

            _pathsChecked.Add(path);



            var msg = new StringBuilder();
            msg.AppendFormat("Couldn't find the '{0}' file. Tried looking in:", ProfileFiles.BottlesManifestFile);
            msg.AppendLine();
            _pathsChecked.Each(p => msg.AppendFormat("  {0}\n", p));
            

            throw new Exception(msg.ToString());
        }
    }

    public class Profile
    {
        private IFileSystem _fileSystem;
        private string _pathToProfile;

        public Profile(IFileSystem fileSystem, string pathToProfile)
        {
            _fileSystem = fileSystem;
            _pathToProfile = pathToProfile;
        }

        public AliasRegistry Aliases()
        {
            var reg = _fileSystem
                .LoadFromFile<AliasRegistry>(AliasRegistry.ALIAS_FILE);

            return reg;
        }
    }
}