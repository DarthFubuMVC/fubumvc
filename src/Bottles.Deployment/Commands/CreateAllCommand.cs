using System;
using System.ComponentModel;
using Bottles.Commands;
using Bottles.Creation;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{
    public class CreateAllInput
    {
        public CreateAllInput()
        {
            TargetFlag = CompileTargetEnum.debug;
        }

        [Description("Overrides the deployment directory")]
        public string DeploymentDirFlag { get; set; }

        [Description("Includes any matching .pdb files for the package assemblies")]
        public bool PdbFlag { get; set; }

        [Description("Overrides the compilation target.  The default is debug")]
        public CompileTargetEnum TargetFlag { get; set; }
    }

    [CommandDescription("Creates all the packages for the directories / manifests listed in the bottles.manifest file and puts the new packages into the deployment/bottles directory")]
    public class CreateAllCommand : FubuCommand<CreateAllInput>
    {
        public override bool Execute(CreateAllInput input)
        {
            return Execute(new FileSystem(), input);

        }

        public bool Execute(IFileSystem system, CreateAllInput input)
        {
            string deploymentDirectory = input.DeploymentDirFlag ?? ProfileFiles.DeploymentFolder;

            var bottlesDirectory = FileSystem.Combine(deploymentDirectory, ProfileFiles.BottlesDirectory);

            Console.WriteLine("Creating all packages");
            Console.WriteLine("  Removing all previous package files");
            system.CleanDirectory(bottlesDirectory);

            var bottleManifestFile = FileSystem.Combine(deploymentDirectory, ProfileFiles.BottlesManifestFile);
            system.ReadTextFile(bottleManifestFile, dir => createPackage(dir, bottlesDirectory, input));

            return true;
        }

        private static void createPackage(string packageFolder, string bottlesDirectory, CreateAllInput input)
        {
            if (packageFolder.IsEmpty()) return;

            Console.WriteLine("  Creating package at " + packageFolder);

            var createInput = new CreatePackageInput(){
                PackageFolder = packageFolder,
                PdbFlag = input.PdbFlag,
                TargetFlag = input.TargetFlag,
                BottlesDirectory = bottlesDirectory
            };

            new CreatePackageCommand().Execute(createInput);
        }
    }
}