using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{
    public class AddBottleInput
    {
        [Description("Name of the bottle")]
        public string Bottle { get; set; }

        [Description("Where the /deployment folder is")]
        public string ProfileFlag { get; set; }

        public string ProfileLocation()
        {
            return ProfileFlag ?? ".".ToFullPath();
        }
    }


    [CommandDescription("Adds the bottle (alias) to the bottles.manifest", Name="add-bottle")]
    public class AddBottleCommand : FubuCommand<AddBottleInput>
    {

        public override bool Execute(AddBottleInput input)
        {
            var fs = new FileSystem();

            var finder = new DeploymentFolderFinder(fs);

            var deploymentDirectory = finder.FindDeploymentFolder(input.ProfileLocation());

            var bottleManifestFile = FileSystem.Combine(deploymentDirectory, ProfileFiles.BottlesManifestFile);

            fs.AppendStringToFile(bottleManifestFile, input.Bottle);

            return true;
        }
    }
}