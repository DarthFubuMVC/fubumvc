using System.Collections.Generic;
using System.Linq;
using Bottles.Configuration;
using FubuCore;

namespace Bottles.Deployment.Parsing
{
    public class ProfileReader : IProfileReader
    {
        private readonly IRecipeSorter _sorter;
        private readonly DeploymentSettings _settings;
        private readonly IFileSystem _fileSystem;

        public ProfileReader(IRecipeSorter sorter, DeploymentSettings settings, IFileSystem fileSystem)
        {
            _sorter = sorter;
            _fileSystem = fileSystem;
            _settings = settings;
        }

        // TODO -- recipe selection / filtering
        public IEnumerable<HostManifest> Read()
        {
            var environment = EnvironmentSettings.ReadFrom(_settings.EnvironmentFile);



            var recipes = RecipeReader.ReadRecipes(_settings.RecipesDirectory, environment);
            recipes = _sorter.Order(recipes);

            // TODO -- throw up if no recipes
            //REVIEW: hardening
            if (recipes == null || !recipes.Any())
                return new HostManifest[0];
            //hardening

            var firstRecipe = recipes.First();
            recipes.Skip(1).Each(firstRecipe.AppendBehind);

            var hosts = firstRecipe.Hosts;

            addEnvironmentSettingsToHosts(environment, hosts);

            
            return hosts;
        }

        private static void addEnvironmentSettingsToHosts(EnvironmentSettings environment, IEnumerable<HostManifest> hosts)
        {
            hosts.Each(host => host.RegisterSettings(environment.DataForHost(host.Name)));
        }
    }
}