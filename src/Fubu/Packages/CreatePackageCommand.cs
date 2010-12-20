using System;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;

namespace Fubu.Packages
{
    [CommandDescription("Create a package file from a package directory", Name = "create-pak")]
    public class CreatePackageCommand : FubuCommand<CreatePackageInput>
    {
        public override void Execute(CreatePackageInput input)
        {
            input.PackageFolder = AliasCommand.AliasFolder(input.PackageFolder);
            Execute(input, new FileSystem());
        }

        public void Execute(CreatePackageInput input, IFileSystem fileSystem)
        {
            if (fileSystem.FileExists(input.ZipFile) && !input.ForceFlag)
            {
                WriteZipFileAlreadyExists(input.ZipFile);
                return;
            }

            // Delete the file if it exists?
            if (fileSystem.PackageManifestExists(input.PackageFolder))
            {
                fileSystem.DeleteFile(input.ZipFile);
                CreatePackage(input, fileSystem);
            }
            else
            {
                WritePackageManifestDoesNotExist(input.PackageFolder);
            }
        }

        public virtual void WriteZipFileAlreadyExists(string zipFileName)
        {
            Console.WriteLine("Package Zip file already exists at {0}.  Use the -force flag to overwrite the existing flag", zipFileName);
        }

        public virtual void WritePackageManifestDoesNotExist(string packageFolder)
        {
            Console.WriteLine(
                "The requested package folder at {0} does not have a package manifest.  Run 'fubu init-pak \"{0}\"' first.",
                packageFolder);
        }

        public virtual void CreatePackage(CreatePackageInput input, IFileSystem fileSystem)
        {
            var manifest = fileSystem.LoadPackageManifestFrom(input.PackageFolder);

            var creator = new PackageCreator(fileSystem, new ZipFileService(), new PackageLogger(), new AssemblyFileFinder(new FileSystem()));
            creator.CreatePackage(input, manifest);
        }
    }
}