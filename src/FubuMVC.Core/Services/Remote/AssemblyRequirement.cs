using System;
using System.IO;
using System.Reflection;
using FubuCore;

namespace FubuMVC.Core.Services.Remote
{
    public class AssemblyRequirement
    {
        private readonly static FileSystem fileSystem = new FileSystem();
        private readonly Assembly _assembly;
        private readonly AssemblyCopyMode _copyMode;

        public AssemblyRequirement(string name)
            : this(name, AssemblyCopyMode.Once)
        {
        }

        public AssemblyRequirement(string name, AssemblyCopyMode copyMode)
        {
            _copyMode = copyMode;
            _assembly = Assembly.Load(name);
        }

        public AssemblyRequirement(Assembly assembly)
            : this(assembly, AssemblyCopyMode.Once)
        {
        }

        public AssemblyRequirement(Assembly assembly, AssemblyCopyMode copyMode)
        {
            _copyMode = copyMode;
            _assembly = assembly;
        }

        public bool ShouldCopyFile(string fileName, string directory)
        {
            if (!fileSystem.FileExists(fileName)) return true;

            if (_copyMode == AssemblyCopyMode.Always) return true;

            if (_copyMode == AssemblyCopyMode.SemVerCompatible && Path.GetExtension(fileName) == ".dll")
            {
                var sourceVersion = _assembly.GetName().Version;
                var currentVersion = AssemblyVersionOf(fileName);

                if (!IsSemVerCompatible(sourceVersion, currentVersion))
                {
                    var template = "The versions of {0} in the source {1} ({2}) and destination directory {3} ({4}) are incompatible";
                    throw new Exception(template.ToFormat(_assembly.GetName().Name, _assembly.Location, sourceVersion.ToString(), directory, currentVersion.ToString()));
                }
            }

            return false;
        }

        public void Move(string directory)
        {
            var location = _assembly.Location;
            var fileName = Path.GetFileName(location);

            var filePath = directory.AppendPath(fileName);
            if (ShouldCopyFile(filePath, directory))
            {
                Console.WriteLine("Copying {0} to {1}", location, directory);
                fileSystem.CopyToDirectory(location, directory);
            }


            var pdb = Path.GetFileNameWithoutExtension(fileName) + ".pdb";
            var pdbPath = directory.AppendPath(Path.GetFileName(pdb));
            if (fileSystem.FileExists(pdb) && ShouldCopyFile(pdbPath, directory))
            {
                fileSystem.CopyToDirectory(location.ParentDirectory().AppendPath(pdb), directory);
            }
        }

        public static bool IsSemVerCompatible(string source, string destination)
        {
            
            var sourceVersion = Version.Parse(source);
            var destinationVersion = Version.Parse(destination);

            return IsSemVerCompatible(sourceVersion, destinationVersion);
        }

        public static bool IsSemVerCompatible(Version sourceVersion, Version destinationVersion)
        {
            if (sourceVersion.Major != destinationVersion.Major)
            {
                return false;
            }

            return (sourceVersion.Minor <= destinationVersion.Minor);
        }

        public static Version AssemblyVersionOf(string filePath)
        {
            return Assembly.LoadFrom(filePath).GetName().Version;
        }
    }

}