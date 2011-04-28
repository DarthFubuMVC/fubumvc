using System.ComponentModel;
using Bottles.Deployment.Bootstrapping;
using Bottles.Deployment.Runtime;
using FubuCore.CommandLine;
using FubuCore;

namespace Bottles.Deployment.Commands
{
    public class DeployInput
    {
        [Description("Path to where the deployment folder is ~/deployment")]
        public string DeploymentFlag { get; set; }

        [FlagAlias("f")]
        public bool ForceFlag { get; set; }

        public string RootDirectory()
        {
            return DeploymentFlag ?? ".".ToFullPath();
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