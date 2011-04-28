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

        [Description("Overrides the deployment directory ~/deployment")]
        public string DeploymentFlag { get; set; }

        [Description("Includes any matching .pdb files for the package assemblies")]
        public bool PdbFlag { get; set; }

        [Description("Overrides the compilation target.  The default is debug")]
        public CompileTargetEnum TargetFlag { get; set; }

        public string DeploymentRoot()
        {
            string deploymentDirectory = DeploymentFlag ?? ProfileFiles.DeploymentFolder;
            return deploymentDirectory;
        }
    }

    [CommandDescription("Creates all the packages for the directories / manifests listed in the bottles.manifest file and puts the new packages into the deployment/bottles directory", Name="create-all")]
    public class CreateAllCommand : FubuCommand<CreateAllInput>
    {
        public override bool Execute(CreateAllInput input)
        {
            return Execute(new FileSystem(), input);
        }

        public bool Execute(IFileSystem system, CreateAllInput input)
        {
            var settings = new DeploymentSettings(input.DeploymentRoot());
            var bottlesDirectory = settings.BottlesDirectory;

            ConsoleWriter.Write("Creating all packages");
            ConsoleWriter.Write("  Removing all previous package files");
            system.CleanDirectory(bottlesDirectory);

            var bottleManifestFile = settings.BottleManifestFile;
            system.ReadTextFile(bottleManifestFile, dir => createPackage(dir, bottlesDirectory, input));
            
            return true;
        }

        private static void createPackage(string packageFolder, string bottlesDirectory, CreateAllInput input)
        {
            if (packageFolder.IsEmpty()) return;

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