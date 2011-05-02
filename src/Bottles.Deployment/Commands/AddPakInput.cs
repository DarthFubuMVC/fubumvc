using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{
    public class AddPakInput
    {
        [Description("Directory (or alias) of the bottle")]
        public string PackageDirectory { get; set; }

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

            var deploy = input.DeploymentFlag.IsEmpty() ? DeploymentSettings.ForRootDirectory() : new DeploymentSettings(input.DeploymentFlag);

            var bottleManifestFile = deploy.BottleManifestFile;

            Console.WriteLine("Adding directory {0} to the bottles manifest at {1}", input.PackageDirectory, deploy.BottleManifestFile);

            fs.AppendStringToFile(bottleManifestFile, "{0}\n".ToFormat(input.PackageDirectory));

            return true;
        }
    }
}