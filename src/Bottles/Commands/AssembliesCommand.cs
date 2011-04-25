using System;
using System.ComponentModel;
using Bottles.Creation;
using FubuCore;
using FubuCore.CommandLine;
using Bottles;

namespace Bottles.Commands
{
    public enum AssembliesCommandMode
    {
        add,
        remove,
        list
    }

    public class AssembliesInput
    {
        public string Directory { get; set; }
        public string File { get; set; }
        public AssembliesCommandMode Mode { get; set; }
        public bool AllFlag { get; set; }
        public string AssemblyFlag { get; set; }
        public bool OpenFlag { get; set; }

        [IgnoreOnCommandLine]
        public PackageManifest Manifest { get; private set;}

        public string BinariesFolder { get; private set;}

        [Description("Choose the compilation target for any assemblies")]
        public CompileTargetEnum TargetFlag { get; set; }


        public void FindManifestAndBinaryFolders(IFileSystem fileSystem)
        {
            BinariesFolder = fileSystem.FindBinaryDirectory(Directory, TargetFlag);

            if (File.IsNotEmpty())
            {
                File = FileSystem.Combine(Directory, File);
                Manifest = fileSystem.LoadFromFile<PackageManifest>(File);
            }

            tryFindManifest(fileSystem, PackageManifest.FILE);
            tryFindManifest(fileSystem, PackageManifest.APPLICATION_MANIFEST_FILE);
        }

        private void tryFindManifest(IFileSystem system, string fileName)
        {
            if (Manifest != null) return;

            var path = FileSystem.Combine(Directory, fileName);
            if (system.FileExists(path))
            {
                File = path;
                Manifest = system.LoadFromFile<PackageManifest>(path);
            }
        }
    }

    public class AssembliesCommand : FubuCommand<AssembliesInput>
    {
        public override bool Execute(AssembliesInput input)
        {
            input.Directory = AliasCommand.AliasFolder(input.Directory);

            var fileSystem = new FileSystem();
            input.FindManifestAndBinaryFolders(fileSystem);
            
            Execute(fileSystem, input);

            return true;
        }

        private void Execute(IFileSystem fileSystem, AssembliesInput input)
        {
            // need binary folder
            // need PackageManifest and file name


            throw new NotImplementedException();
        }
    }
}