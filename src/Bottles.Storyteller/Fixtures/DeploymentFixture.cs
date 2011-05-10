using System;
using System.Collections.Generic;
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
        [FormatAs("Profile is {profile}")]
        public void ProfileIs(string profile)
        {
            throw new NotImplementedException();
        }

        [FormatAs("Recipes are {recipes}")]
        public void Recipes(string[] recipes)
        {
            throw new NotImplementedException();
        }
    }

    public class DeploymentConfigurationFixture : Fixture
    {
        [ExposeAsTable("The environment settings are")]
        public void EnvironmentSettings(string Key, string Value)
        {
            throw new NotImplementedException();
        }

        [ExposeAsTable("The profile settings are")]
        public void ProfileSettings(string ProfileName, string Key, string Value)
        {
            throw new NotImplementedException();
        }

        [FormatAs("Profile {profile} contains recipe(s) {recipeNames}")]
        public void ProfileRecipes(string profile, string[] recipeNames)
        {
            throw new NotImplementedException();
        }

        [FormatAs("Recipe {recipe} depends on other recipe(s) {dependencies}")]
        public void RecipeDependencies(string recipe, string[] dependencies)
        {
            throw new NotImplementedException();
        }

        [ExposeAsTable("The host directive values are")]
        public void HostValues(string Recipe, string Host, string Key, string Value)
        {
            throw new NotImplementedException();
        }
    }



    public class ProfileReaderFixture : Fixture
    {
        private IEnumerable<SettingDataSource> _hostData;


        [FormatAs("All the properties for host {host} are")]
        public void FetchPropertiesForHost(string host)
        {
            throw new NotImplementedException();            
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
            throw new NotImplementedException();
        }


    }


}