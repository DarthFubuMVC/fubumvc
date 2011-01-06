using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;

namespace Fubu
{
    public class ManifestInput
    {
        [Description("Physical folder (or valid alias) of the application")]
        public string AppFolder { get; set; }

        [Description("Opens the manifest file in notepad")]
        public bool OpenFlag { get; set; }

        [Description("Creates a new application manifest file for the application")]
        public bool CreateFlag { get; set; } // creates, but does not override

        [Description("Force the command to overwrite any existing manifest file if using the -create flag")]
        [FlagAlias("f")]
        public bool ForceFlag { get; set; }

        [Description("Write the main application assembly to the manifest file for usage with the install command")]
        public string AssemblyFlag { get; set; }

        [Description("Write the main environment class name in assembly qualified form for usage with the install command")]
        [FlagAlias("class")]
        public string EnvironmentClassNameFlag { get; set; }
    }

    [CommandDescription("Access and modify an application manifest file")]
    public class ManifestCommand : FubuCommand<ManifestInput>
    {
        public override void Execute(ManifestInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder);
            Execute(input, new FileSystem());
        }


        public virtual bool ApplyChanges(ManifestInput input, ApplicationManifest manifest)
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

        public void Execute(ManifestInput input, IFileSystem fileSystem)
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
                    fileSystem.LaunchEditor(input.AppFolder, ApplicationManifest.FILE);
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
                }
            }
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
            var manifest = new ApplicationManifest();
            ApplyChanges(input, manifest);
            persist(fileSystem, input, manifest);

            WriteManifest(input, manifest);

            if (input.OpenFlag)
            {
                fileSystem.LaunchEditor(input.AppFolder, ApplicationManifest.FILE);
            }
        }

        private void persist(IFileSystem fileSystem, ManifestInput input, ApplicationManifest manifest)
        {
            Console.WriteLine("");
            Console.WriteLine("Persisted changes to " + FileSystem.Combine(input.AppFolder, ApplicationManifest.FILE));
            Console.WriteLine("");

            fileSystem.PersistToFile(manifest, input.AppFolder, ApplicationManifest.FILE);
        }

        public virtual void WriteManifest(ManifestInput input, ApplicationManifest manifest)
        {
            var title = "Application Manifest for " + FileSystem.Combine(input.AppFolder, ApplicationManifest.FILE);
            var report = new TwoColumnReport(title);
            report.Add<ApplicationManifest>(x => x.EnvironmentAssembly, manifest);
            report.Add<ApplicationManifest>(x => x.EnvironmentClassName, manifest);
            report.Add<ApplicationManifest>(x => x.ConfigurationFile, manifest);

            report.Write();

            Console.WriteLine();
            Console.WriteLine();

            LinkCommand.ListCurrentLinks(input.AppFolder, manifest);
        }

        public virtual void WriteManifestCannotBeFound(string folder)
        {
            var file = FileSystem.Combine(folder, ApplicationManifest.FILE);
            Console.WriteLine("Application Manifest file at {0} does not exist", file);
        }

        public virtual void WriteCannotOverwriteFileWithoutForce(string folder)
        {
            var file = FileSystem.Combine(folder, ApplicationManifest.FILE);
            Console.WriteLine("File {0} already exists, use the '-f' flag to overwrite the existing file", file);
        }
    }
}