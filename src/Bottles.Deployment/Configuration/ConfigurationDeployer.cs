using System;
using System.IO;
using FubuCore;

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
        private readonly IFileSystem _fileSystem;
        private readonly IBottleRepository _repository;

        public ConfigurationDeployer(DeploymentSettings deploymentSettings, IFileSystem fileSystem, IBottleRepository repository)
        {
            _deploymentSettings = deploymentSettings;
            _fileSystem = fileSystem;
            _repository = repository;
        }

        public void Deploy(HostManifest manifest, IDirective directive)
        {
            var configDirective = (ConfigurationDirective)directive;

            // copy the environment settings there
            // explode the bottles out to there
            // log each file to there


            var configDirectory = configDirective.ConfigDirectory.CombineToPath(_deploymentSettings.TargetDirectory);
            _fileSystem.Copy(_deploymentSettings.EnvironmentFile, configDirectory);
                
            


            throw new NotImplementedException();
        }
    }
}