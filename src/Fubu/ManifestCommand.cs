using System;
using Bottles;
using Bottles.Commands;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu
{
    [CommandDescription("Show the contents of the .package-manifest file.")]
    public class ManifestCommand : FubuCommand<ManifestInput>
    {
        public override bool Execute(ManifestInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder);
            return Execute(input, new FileSystem());
        }


        public bool Execute(ManifestInput input, IFileSystem fileSystem)
        {
            if (fileSystem.ApplicationManifestExists(input.AppFolder))
            {
                modifyAndListExistingManifest(fileSystem, input);

                if (input.OpenFlag)
                {
                    fileSystem.LaunchEditor(input.AppFolder, PackageManifest.FILE);
                }
            }
            else
            {
                WriteManifestCannotBeFound(input.AppFolder);
                return false;
            }
            return true;
        }

        private void modifyAndListExistingManifest(IFileSystem fileSystem, ManifestInput input)
        {
            var manifest = fileSystem.LoadApplicationManifestFrom(input.AppFolder);
            WriteManifest(input, manifest);
        }

        public virtual void WriteManifest(ManifestInput input, PackageManifest manifest)
        {
            var title = "Application Manifest for " + FileSystem.Combine(input.AppFolder, PackageManifest.FILE);
            var report = new TwoColumnReport(title);
            report.Add<PackageManifest>(x => x.EnvironmentAssembly, manifest);
            report.Add<PackageManifest>(x => x.EnvironmentClassName, manifest);
            report.Add<PackageManifest>(x => x.ConfigurationFile, manifest);

            report.Write();

            Console.WriteLine();
            Console.WriteLine();

            LinkCommand.ListCurrentLinks(input.AppFolder, manifest);
        }

        public virtual void WriteManifestCannotBeFound(string folder)
        {
            var file = FileSystem.Combine(folder, PackageManifest.FILE);
            Console.WriteLine("Application Manifest file at {0} does not exist", file);
        }

        public virtual void WriteCannotOverwriteFileWithoutForce(string folder)
        {
            var file = FileSystem.Combine(folder, PackageManifest.FILE);
            Console.WriteLine("File {0} already exists, use the '-f' flag to overwrite the existing file", file);
        }
    }
}