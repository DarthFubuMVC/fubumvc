using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace Bottles.Deployment.Parsing
{
    public class ProfileReader : IProfileReader
    {
        private readonly IRecipeSorter _sorter;
        private readonly DeploymentSettings _settings;

        public ProfileReader(IRecipeSorter sorter, DeploymentSettings settings)
        {
            _sorter = sorter;
            _settings = settings;
        }

        public IEnumerable<HostManifest> Read()
        {
            var environment = new EnvironmentSettings();
            new FileSystem().ReadTextFile(_settings.EnvironmentFile, environment.ReadText);

            var recipes = RecipeReader.ReadRecipes(_settings.RecipesDirectory);
            recipes = _sorter.Order(recipes);

            // TODO -- harden.  Must be at least 1 recipe
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