using System.Diagnostics;
using Bottles.Deployment;
using Bottles.Deployment.Parsing;
using Bottles.Deployment.Writing;
using Bottles.Tests.Deployment.Writing;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using FubuCore;

namespace Bottles.Tests.Deployment.Parsing
{

    public class OneSettings : IDirective
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TwoSettings : IDirective
    {
        public string City { get; set; }
        public bool IsDomestic { get; set; }
    }

    public class ThreeSettings : IDirective
    {
        public int Threshold { get; set; }
        public string Direction { get; set; }
    }

    [TestFixture]
    public class when_reading_a_recipe_with_multiple_hosts
    {
        private Recipe theRecipe;

        [SetUp]
        public void SetUp()
        {
            var writer = new ProfileWriter("profile3");

            var recipeDefinition = writer.RecipeFor("r1");
            var host = recipeDefinition.HostFor("h1");

            host.AddDirective(new SimpleSettings
            {
                One = "one",
                Two = "two"
            });

            host.AddDirective(new OneSettings()
            {
                Name = "Jeremy",
                Age = 37
            });

            host.AddReference(new BottleReference()
            {
                Name = "bottle1"
            });

            host.AddReference(new BottleReference()
            {
                Name = "bottle2",
                Relationship = "binaries"
            });

            recipeDefinition.HostFor("h2").AddProperty<ThreeSettings>(x => x.Direction, "North");
            recipeDefinition.HostFor("h3").AddProperty<TwoSettings>(x => x.City, "Austin");


            writer.Flush();


            theRecipe = RecipeReader.ReadFrom("profile3/recipes/r1");
        }

        [Test]
        public void should_read_the_recipe_name_from_the_folder_name()
        {
            theRecipe.Name.ShouldEqual("r1");
        }

        [Test]
        public void should_have_all_three_hosts()
        {
            theRecipe.Hosts.Select(x => x.Name).ShouldHaveTheSameElementsAs("h1", "h2", "h3");
        }

        [Test]
        public void spot_check_that_the_host_has_the_settings_data()
        {
            theRecipe.HostFor("h1").As<HostManifest>().AllSettingsData().Single()
                .Get("OneSettings.Name").ShouldEqual("Jeremy");
        }
    }
}