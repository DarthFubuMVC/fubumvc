using System;

namespace Bottles.Deployment.Configuration
{
    public class ConfigurationDirective : IDirective
    {
        public ConfigurationDirective()
        {
            ConfigDirectory = "config";
        }

        public string ConfigDirectory { get; set;}
    }

    public class ConfigurationDeployer : IDeployer<ConfigurationDirective>
    {
        private readonly DeploymentSettings _deploymentSettings;

        public ConfigurationDeployer(DeploymentSettings deploymentSettings)
        {
            _deploymentSettings = deploymentSettings;
        }

        public void Deploy(IDirective directive)
        {
            // copy the environment settings there
            // explode the bottles out to there
            // log each file to there

            throw new NotImplementedException();
        }
    }
}