using System;
using System.Collections.Generic;
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

        [Description("Path to the deployment folder (~/deployment)")]
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
            var deploymentSettings = DeploymentSettings.ForDirectory(input.DeploymentFlag);

            var settings = new EnvironmentSettings();

            IFileSystem fileSystem = new FileSystem();

            Execute(input, settings, fileSystem, deploymentSettings);

            return true;
        }

        public void Execute(AddReferenceCommandInput input, EnvironmentSettings settings, IFileSystem fileSystem, DeploymentSettings deploymentSettings)
        {
            string bottleText = "bottle:{0}".ToFormat(input.Bottle);

            
            var hostPath = deploymentSettings.GetHost(input.Recipe, input.Host);
            Console.WriteLine("Analyzing the host file at " + input.Host);
            fileSystem.AlterFlatFile(hostPath, list =>
            {
                list.Fill(bottleText);
                list.Sort();

                Console.WriteLine("Contents of file " + hostPath);
                list.Each(x => Console.WriteLine("  " + x));
            });
        }

        
    }
}