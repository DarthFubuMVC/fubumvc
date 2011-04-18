using System.Collections.Generic;
using System.IO;
using FubuCore;
using System.Linq;

namespace Bottles.Deployment.Parsing
{
    public static class ProfileReader
    {
        public static IEnumerable<Recipe> ReadRecipes(string profileDirectory)
        {
            var recipesDir = FileSystem.Combine(profileDirectory, ProfileFiles.RecipesFolder);
            return Directory.GetDirectories(recipesDir).Select(RecipeReader.ReadFrom);
        }
    }
}