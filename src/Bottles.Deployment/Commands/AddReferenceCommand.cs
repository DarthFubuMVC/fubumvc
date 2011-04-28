using System.ComponentModel;
using Bottles.Configuration;
using Bottles.Deployment.Parsing;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{
    public class AddReferenceCommandInput
    {
        [Description("The recipe that the host is in")]
        public string Recipe { get; set; }

        [Description("The host to add the reference to")]
        public string Host { get; set; }

        [Description("The name of the bottle to link")]
        public string Bottle { get; set; }

        [Description("Set the relationship of the bottle")]
        public string RelationshipFlag { get; set; }

        [Description("Path to the deployment folder")]
        public string DeploymentFlag { get; set; }

        public string DeploymentRoot()
        {
            return DeploymentFlag ?? ".".ToFullPath();
        }
    }


    [CommandDescription("Adds a bottles reference to the specified host", Name="ref")]
    public class AddReferenceCommand : FubuCommand<AddReferenceCommandInput>
    {
        public override bool Execute(AddReferenceCommandInput input)
        {
            var sett = new DeploymentSettings(input.DeploymentRoot());

            var settings = new EnvironmentSettings();

            IFileSystem fileSystem = new FileSystem();

            Exe(input, settings, fileSystem, sett);

            return true;
        }

        public void Exe(AddReferenceCommandInput input, EnvironmentSettings settings, IFileSystem fileSystem, DeploymentSettings sett)
        {
            var hostPath = sett.GetHost(input.Recipe, input.Host);

            var hostManifest = HostReader.ReadFrom(hostPath, settings);

            if (!hostManifest.HasBottle(input.Bottle))
                fileSystem.AppendStringToFile(hostPath, "bottle:{0} {1}".ToFormat(input.Bottle, input.RelationshipFlag));
        }

        
    }
}