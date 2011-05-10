using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bottles.Deployment;
using Bottles.Deployment.Parsing;
using Bottles.Deployment.Runtime;
using Bottles.Deployment.Writing;
using FubuCore;
using FubuCore.Configuration;
using StoryTeller;
using StoryTeller.Engine;

namespace Bottles.Storyteller.Fixtures
{
    public class DeploymentFixture : Fixture
    {
        public DeploymentFixture()
        {
            Title = "Bottles Deployment";

            this["Configure"] = Embed<DeploymentConfigurationFixture>("If the deployment configuration is");
            this["Options"] = Embed<DeploymentOptionsFixture>("And the deployment options are");
            this["ReadingProfile"] = Embed<ProfileReaderFixture>("Then reading the deployment profile results in");
        }
    }

   

    public class DeploymentOptionsFixture : Fixture
    {
        private DeploymentOptions _options;

        public override void SetUp(ITestContext context)
        {
            _options = new DeploymentOptions();
            context.Store(_options);
        }

        [FormatAs("Profile is {profile}")]
        public void ProfileIs(string profile)
        {
            _options.ProfileName = profile;
        }

        [FormatAs("Recipes are {recipes}")]
        public void Recipes(string[] recipes)
        {
            _options.RecipeNames.AddRange(recipes);
        }
    }

    public class DeploymentConfigurationFixture : Fixture
    {
        private DeploymentSettings _settings;
        private DeploymentWriter _writer;

        public override void SetUp(ITestContext context)
        {
            _settings = new DeploymentSettings("storyteller");
            context.Store(_settings);

            _writer = new DeploymentWriter("storyteller");
        }

        public override void TearDown()
        {
            Debug.WriteLine("Writing to " + _settings.DeploymentDirectory.ToFullPath());
            _writer.Flush(FlushOptions.Wipeout);
        }


        [ExposeAsTable("The environment settings are")]
        public void EnvironmentSettings(string Key, string Value)
        {
            _writer.AddEnvironmentSetting(Key, Value);
        }

        [ExposeAsTable("The profile settings are")]
        public void ProfileSettings(string ProfileName, string Key, string Value)
        {
            _writer.ProfileFor(ProfileName).AddProperty(Key, Value);
        }

        [FormatAs("Profile {profile} contains recipe(s) {recipeNames}")]
        public void ProfileRecipes(string profile, string[] recipeNames)
        {
            
            var profileDef = _writer.ProfileFor(profile);
            recipeNames.Each(profileDef.AddRecipe);
        }

        [FormatAs("Recipe {recipe} depends on other recipe(s) {dependencies}")]
        public void RecipeDependencies(string recipe, string[] dependencies)
        {
            var recipeDef = _writer.RecipeFor(recipe);
            dependencies.Each(recipeDef.RegisterDependency);
        }

        [ExposeAsTable("The host directive values are")]
        public void HostValues(string Recipe, string Host, string Key, string Value)
        {
            _writer.RecipeFor(Recipe).HostFor(Host).AddProperty(Key, Value);
        }
    }



    public class ProfileReaderFixture : Fixture
    {
        private IEnumerable<SettingDataSource> _hostData;
        private DeploymentSettings _deploymentSettings;
        private DeploymentOptions _deploymentOptions;
        private ProfileReader _profileReader;

        public override void SetUp(ITestContext context)
        {
            _deploymentSettings = context.Retrieve<DeploymentSettings>();
            _deploymentOptions = context.Retrieve<DeploymentOptions>();
            _profileReader = new ProfileReader(new RecipeSorter(), _deploymentSettings, new FileSystem());
        }

        [FormatAs("All the properties for host {host} are")]
        public void FetchPropertiesForHost(string host)
        {
            _hostData = _profileReader.Read(_deploymentOptions)
                .Single(h => h.Name == host).CreateDiagnosticReport();
        }

        public IGrammar CheckPropertiesForHost()
        {
            return VerifySetOf(() => _hostData)
                .Titled("The deployment properties")
                .MatchOn(x => x.Key, x => x.Value, x => x.Provenance);
        }

        public IGrammar CheckHosts()
        {
            return VerifyStringList(findHosts)
                .Titled("The hosts in order are")
                .Ordered()
                .Grammar();
        }

        private IEnumerable<string> findHosts()
        {
            return _profileReader.Read(_deploymentOptions).Select(h=>h.Name);
        }


    }


}