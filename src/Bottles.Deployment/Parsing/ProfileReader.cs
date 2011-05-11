using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.Configuration;
using Bottles.Deployment.Runtime;
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

        // TODO
        //         -- unless Recipes is non-empty, then use that one
        //         -- fill in the recipes
        public static IEnumerable<Recipe> FilterRecipes(Profile profile, DeploymentOptions options, IEnumerable<Recipe> recipes)
        {
            return recipes.Where(r => profile.Recipes.Contains(r.Name));
        }

        public Profile ReadProfile(DeploymentOptions options, EnvironmentSettings settings)
        {
            var profileFile = _settings.GetProfile(options.ProfileName);
            var profile = new Profile(settings);

            _fileSystem.ReadTextFile(profileFile, profile.ReadText);

            options.RecipeNames.Each(profile.AddRecipe);

            return profile;
        }

        public IEnumerable<HostManifest> Read(DeploymentOptions options)
        {
            var environment = EnvironmentSettings.ReadFrom(_settings.EnvironmentFile);
            environment.SetRoot(_settings.TargetDirectory);

            var profile = ReadProfile(options, environment);

            var recipes = readRecipes(environment, options, profile);
            var hosts = collateHosts(recipes);

            addEnvironmentSettingsToHosts(environment, hosts);
            
            return hosts;
        }

        private IEnumerable<HostManifest> collateHosts(IEnumerable<Recipe> recipes)
        {
            // TODO -- throw up if no recipes
            //REVIEW: hardening
            if (recipes == null || !recipes.Any())
                return new HostManifest[0];
            //hardening

            var firstRecipe = recipes.First();
            recipes.Skip(1).Each(firstRecipe.AppendBehind);

            return firstRecipe.Hosts;
        }

        private IEnumerable<Recipe> readRecipes(EnvironmentSettings environment, DeploymentOptions options, Profile profile)
        {
            // TODO -- log which recipes are read
            var recipes = RecipeReader.ReadRecipes(_settings.RecipesDirectory, environment);
            recipes = FilterRecipes(profile, options, recipes);
            recipes = _sorter.Order(recipes);
            return recipes;
        }

        private static void addEnvironmentSettingsToHosts(EnvironmentSettings environment, IEnumerable<HostManifest> hosts)
        {
            hosts.Each(host => host.RegisterSettings(environment.DataForHost(host.Name)));
        }
    }
}