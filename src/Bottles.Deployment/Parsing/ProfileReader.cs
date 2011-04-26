using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<HostManifest> Read()
        {
            var environment = new EnvironmentSettings();
            _fileSystem.ReadTextFile(_settings.EnvironmentFile, environment.ReadText);

            var recipes = RecipeReader.ReadRecipes(_settings.RecipesDirectory, environment);
            recipes = _sorter.Order(recipes);

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