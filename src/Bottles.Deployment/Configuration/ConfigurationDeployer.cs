using System;
using System.IO;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuCore;
using System.Collections.Generic;

namespace Bottles.Deployment.Configuration
{
    public class ConfigurationDirectory : IDirective
    {
        public ConfigurationDirectory()
        {
            ConfigDirectory = "config";
        }

        public string ConfigDirectory { get; set;}
    }

    public class ConfigurationDeployer : IDeployer<ConfigurationDirectory>
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


        public void Execute(ConfigurationDirectory directive, HostManifest host, IPackageLog log)
        {
            // copy the environment settings there
            // explode the bottles out to there
            // log each file to there

            var configDirectory = directive.ConfigDirectory.CombineToPath(_deploymentSettings.TargetDirectory);
            _fileSystem.Copy(_deploymentSettings.EnvironmentFile, configDirectory);

            var destinationDirectory = FileSystem.Combine(_deploymentSettings.TargetDirectory,
                                                          directive.ConfigDirectory);

            // TODO -- diagnostics?
            host.BottleReferences.Each(x =>
            {
                var request = new BottleExplosionRequest(new PackageLog())
                {
                    BottleDirectory = BottleFiles.ConfigFolder,
                    BottleName = x.Name,
                    DestinationDirectory = destinationDirectory
                };

                _repository.ExplodeFiles(request);
            });
        }
    }
}