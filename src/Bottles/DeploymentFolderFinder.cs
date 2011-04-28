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

    public class DeploymentFolderFinder : IProfileFinder
    {
        private readonly IFileSystem _fileSystem;
        private readonly IList<string> _pathsChecked;

        public DeploymentFolderFinder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _pathsChecked = new List<string>();
        }

        public string FindDeploymentFolder(string startPath)
        {
            var path = FileSystem.Combine(startPath, ProfileFiles.BottlesManifestFile);
            if (_fileSystem.FileExists(path))
            {
                path = _fileSystem.GetDirectory(path);
                ConsoleWriter.Write("Found deployment folder at {0}", path);
                return path;
            }
                

            _pathsChecked.Add(path);


            path = FileSystem.Combine(startPath, ProfileFiles.DeploymentFolder, ProfileFiles.BottlesManifestFile);
            if (_fileSystem.FileExists(path))
            {
                path = _fileSystem.GetDirectory(path);
                ConsoleWriter.Write("Found deployment folder at {0}", path);
                return path;
            }

            _pathsChecked.Add(path);



            var msg = new StringBuilder();
            msg.AppendFormat("Couldn't find the '{0}' file. Tried looking in:", ProfileFiles.BottlesManifestFile);
            msg.AppendLine();
            _pathsChecked.Each(p => msg.AppendFormat("  {0}\n", p));
            

            throw new Exception(msg.ToString());
        }
    }
}