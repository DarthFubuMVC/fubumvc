using System;
using System.ComponentModel;
using Bottles.Deployment.Bootstrapping;
using Bottles.Deployment.Runtime;
using FubuCore.CommandLine;
using FubuCore;

namespace Bottles.Deployment.Commands
{
    public class DeployInput
    {
        [Description("Physical path to where the deployment is")]
        public string DirectoryFlag { get; set; }

        [FlagAlias("f")]
        public bool ForceFlag { get; set; }

        public string RootDirectory()
        {
            return DirectoryFlag ?? ".".ToFullPath();
        }

    }

    [CommandDescription("Deploys the given profile")]
    public class DeployCommand : FubuCommand<DeployInput>
    {
        public override bool Execute(DeployInput input)
        {
            var settings = new DeploymentSettings(input.RootDirectory())
            {
                UserForced = input.ForceFlag
            };

            var container = DeploymentBootstrapper.Bootstrap(settings);
            var deploymentController = container.GetInstance<IDeploymentController>();
            deploymentController.Deploy();
             
            return true;
        }
    }
}