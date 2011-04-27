using System.Collections.Generic;
using System.ComponentModel;
using Bottles.Deployment.Parsing;
using Bottles.Deployment.Runtime;
using Bottles.Deployment.Writing;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Reflection;

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

        public string DeployRoot()
        {
            return DeploymentFlag ?? ".".ToFullPath();
        }
    }


    [CommandDescription("Adds a bottles reference to the specified host", Name="ref")]
    public class AddReferenceCommand : FubuCommand<AddReferenceCommandInput>
    {
        public override bool Execute(AddReferenceCommandInput input)
        {
            var settings = new EnvironmentSettings();
            IFileSystem fileSystem = new FileSystem();

            Exe(input, settings, fileSystem, new ProfileFinder(fileSystem));

            return true;
        }

        public void Exe(AddReferenceCommandInput input, EnvironmentSettings settings, IFileSystem fileSystem, IProfileFinder deployFinder)
        {
            var path = deployFinder.FindDeploymentFolder(input.DeployRoot());

            var recipeDirectory = FileSystem.Combine(path, ProfileFiles.RecipesDirectory, input.Recipe);


            //YUCK -> host + ".host"
            var hostPath = FileSystem.Combine(recipeDirectory, input.Host + ".host");

            var hostManifest = HostReader.ReadFrom(hostPath, settings);

            if (!hostManifest.HasBottle(input.Bottle))
                fileSystem.AppendStringToFile(hostPath, "bottle:{0} {1}".ToFormat(input.Bottle, input.RelationshipFlag));
        }

        
    }
}