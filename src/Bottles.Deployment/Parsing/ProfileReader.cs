using System.Collections.Generic;
using System.Linq;

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
            var recipes = RecipeReader.ReadRecipes(_settings.RecipesDirectory);
            recipes = _sorter.Order(recipes);

            // TODO -- harden.  Must be at least 1 recipe
            var firstRecipe = recipes.First();
            recipes.Skip(1).Each(firstRecipe.AppendBehind);

            // read in the environment file here

            return firstRecipe.Hosts;
        }
    }
}