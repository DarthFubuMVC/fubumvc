using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles.Configuration;
using FubuCore;

namespace Bottles.Deployment.Parsing
{
    public class RecipeReader
    {
        public static Recipe ReadFrom(string recipeDirectory, EnvironmentSettings environment, Profile profile)
        {
            var recipeName = Path.GetFileName(recipeDirectory);
            var recipe = new Recipe(recipeName);
            var fileSystem = new FileSystem();

            fileSystem.ReadTextFile(FileSystem.Combine(recipeDirectory, ProfileFiles.RecipesControlFile), s =>
                {
                    //TODO: Harden this for bad syntax
                    var parts = s.Split(':');
                    recipe.RegisterDependency(parts[1]);
                });


            fileSystem.FindFiles(recipeDirectory, new FileSet(){
                Include = "*.host"
            }).Each(file =>
            {
                var host = HostReader.ReadFrom(file, environment, profile);
                recipe.RegisterHost(host);
            });

            return recipe;
        }

        public static IEnumerable<Recipe> ReadRecipes(string recipesDir, EnvironmentSettings environment, Profile profile)
        {
            return Directory.GetDirectories(recipesDir).Select(dir => ReadFrom(dir, environment, profile));
        }
    }
}