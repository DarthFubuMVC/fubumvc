using System;
using System.IO;
using Bottles.Diagnostics;
using FubuCore;
using System.Collections.Generic;

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

            var destinationDirectory = FileSystem.Combine(_deploymentSettings.TargetDirectory,
                                                          configDirective.ConfigDirectory);
            
            // TODO -- diagnostics?
            manifest.BottleReferences.Each(x =>
            {
                var request = new BottleExplosionRequest(new PackageLog()){
                    BottleDirectory = BottleFiles.ConfigFolder,
                    BottleName = x.Name,
                    DestinationDirectory = destinationDirectory
                };

                _repository.ExplodeFiles(request);
            });
            

        }
    }
}