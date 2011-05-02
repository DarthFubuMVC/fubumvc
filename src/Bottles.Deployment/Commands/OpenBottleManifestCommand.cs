using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{
    public class OpenBottleManifestInput
    {
        [Description("Overrides the deployment directory ~/deployment")]
        public string DirectoryFlag { get; set; }
    }

    [CommandDescription("Open the bottle manifest file", Name = "open-bottles-manifest")]
    public class OpenBottleManifestCommand : FubuCommand<OpenBottleManifestInput>
    {
        public override bool Execute(OpenBottleManifestInput input)
        {
            var settings = DeploymentSettings.ForDirectory(input.DirectoryFlag);

            new FileSystem().LaunchEditor(settings.BottleManifestFile);

            return true;
        }
    }
}