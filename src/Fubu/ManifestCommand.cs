using System;
using Bottles;
using Bottles.Commands;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu
{
    [CommandDescription("Access and modify an application manifest file")]
    public class ManifestCommand : FubuCommand<ManifestInput>
    {
        public override bool Execute(ManifestInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder);
            return Execute(input, new FileSystem());
        }


        public virtual bool ApplyChanges(ManifestInput input, PackageManifest manifest)
        {
            var didChange = false;

            if (input.AssemblyFlag.IsNotEmpty())
            {
                manifest.EnvironmentAssembly = input.AssemblyFlag;
                didChange = true;
            }

            if (input.EnvironmentClassNameFlag.IsNotEmpty())
            {
                manifest.EnvironmentClassName = input.EnvironmentClassNameFlag;
                didChange = true;
            }

            return didChange;
        }

        public bool Execute(ManifestInput input, IFileSystem fileSystem)
        {
            if (fileSystem.ApplicationManifestExists(input.AppFolder))
            {
                if (input.CreateFlag)
                {
                    overwriteExistingFile(fileSystem, input);
                }
                else
                {
                    modifyAndListExistingManifest(fileSystem, input);
                }

                if (input.OpenFlag)
                {
                    fileSystem.LaunchEditor(input.AppFolder, PackageManifest.APPLICATION_MANIFEST_FILE);
                }
            }
            else
            {
                if (input.CreateFlag)
                {
                    CreateManifest(fileSystem, input);
                }
                else
                {
                    WriteManifestCannotBeFound(input.AppFolder);
                    return false;
                }
            }
            return true;
        }

        private void modifyAndListExistingManifest(IFileSystem fileSystem, ManifestInput input)
        {
            var manifest = fileSystem.LoadApplicationManifestFrom(input.AppFolder);
            if (ApplyChanges(input, manifest))
            {
                persist(fileSystem, input, manifest);
            }

            WriteManifest(input, manifest);
        }

        private void overwriteExistingFile(IFileSystem fileSystem, ManifestInput input)
        {
            if (input.ForceFlag)
            {
                CreateManifest(fileSystem, input);
            }
            else
            {
                WriteCannotOverwriteFileWithoutForce(input.AppFolder);
            }
        }

        public virtual void CreateManifest(IFileSystem fileSystem, ManifestInput input)
        {
            var manifest = new PackageManifest();
            ApplyChanges(input, manifest);
            persist(fileSystem, input, manifest);

            WriteManifest(input, manifest);

            if (input.OpenFlag)
            {
                fileSystem.LaunchEditor(input.AppFolder, PackageManifest.APPLICATION_MANIFEST_FILE);
            }
        }

        private void persist(IFileSystem fileSystem, ManifestInput input, PackageManifest manifest)
        {
            Console.WriteLine("");
            Console.WriteLine("Persisted changes to " + FileSystem.Combine(input.AppFolder, PackageManifest.APPLICATION_MANIFEST_FILE));
            Console.WriteLine("");

            fileSystem.PersistToFile(manifest, input.AppFolder, PackageManifest.APPLICATION_MANIFEST_FILE);
        }

        public virtual void WriteManifest(ManifestInput input, PackageManifest manifest)
        {
            var title = "Application Manifest for " + FileSystem.Combine(input.AppFolder, PackageManifest.APPLICATION_MANIFEST_FILE);
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
            var file = FileSystem.Combine(folder, PackageManifest.APPLICATION_MANIFEST_FILE);
            Console.WriteLine("Application Manifest file at {0} does not exist", file);
        }

        public virtual void WriteCannotOverwriteFileWithoutForce(string folder)
        {
            var file = FileSystem.Combine(folder, PackageManifest.APPLICATION_MANIFEST_FILE);
            Console.WriteLine("File {0} already exists, use the '-f' flag to overwrite the existing file", file);
        }
    }
}