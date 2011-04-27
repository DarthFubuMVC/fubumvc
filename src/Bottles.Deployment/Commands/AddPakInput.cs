using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{
    public class AddPakInput
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


    [CommandDescription("Adds the pak (alias) to the bottles.manifest", Name="add-pak")]
    public class AddPakCommand : FubuCommand<AddPakInput>
    {

        public override bool Execute(AddPakInput input)
        {
            var fs = new FileSystem();

            var finder = new ProfileFinder(fs);

            var deploymentDirectory = finder.FindDeploymentFolder(input.ProfileLocation());

            var bottleManifestFile = FileSystem.Combine(deploymentDirectory, ProfileFiles.BottlesManifestFile);

            fs.AppendStringToFile(bottleManifestFile, "{0}\n".ToFormat(input.Bottle));

            return true;
        }
    }
}