using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{
    public class AddPakInput
    {
        [Description("Name of the bottle")]
        public string Bottle { get; set; }

        [Description("Where the ~/deployment folder is")]
        public string DeploymentFlag { get; set; }

        public string DeploymentRoot()
        {
            return DeploymentFlag ?? ".".ToFullPath();
        }
    }


    [CommandDescription("Adds the pak (alias) to the bottles.manifest", Name="add-pak")]
    public class AddPakCommand : FubuCommand<AddPakInput>
    {

        public override bool Execute(AddPakInput input)
        {
            var fs = new FileSystem();

            var deploy = new DeploymentSettings(input.DeploymentRoot());

            var bottleManifestFile = deploy.BottleManifestFile;

            fs.AppendStringToFile(bottleManifestFile, "{0}\n".ToFormat(input.Bottle));

            return true;
        }
    }
}