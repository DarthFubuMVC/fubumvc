using System.Linq;
using Bottles.Deployment;
using Bottles.Deployment.Parsing;
using Bottles.Deployment.Writing;
using Bottles.Tests.Deployment.Writing;
using FubuCore.Configuration;
using FubuTestingSupport;
using NUnit.Framework;

namespace Bottles.Tests.Deployment.Parsing
{
    [TestFixture]
    public class when_reading_a_single_host_file
    {
        private HostManifest theHost;

        [SetUp]
        public void SetUp()
        {
            var writer = new ProfileWriter("profile2");

            var host = writer.RecipeFor("r1").HostFor("h1");
            
            host.AddDirective(new SimpleSettings{
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

            writer.Flush();

            theHost = HostReader.ReadFrom("profile2/recipes/r1/h1.host");
        }

        [Test]
        public void the_category_for_settings_data_on_the_host_must_be_core()
        {
            var settingsData = theHost.AllSettingsData().Single();
            settingsData.Category.ShouldEqual(SettingCategory.core);
        }

        [Test]
        public void should_have_all_the_settings_data_from_the_file()
        {
            var settingsData = theHost.AllSettingsData().Single();

            settingsData.AllKeys.ShouldHaveTheSameElementsAs(
                "SimpleSettings.One",
                "SimpleSettings.Two",
                "OneSettings.Name",
                "OneSettings.Age"
                );

            settingsData.Get("SimpleSettings.One").ShouldEqual("one");
        }

        [Test]
        public void has_all_the_bottle_references_from_the_file()
        {
            theHost.BottleReferences.ShouldHaveTheSameElementsAs(
                new BottleReference("bottle1", null),
                new BottleReference("bottle2", "binaries")
                );
        }

        [Test]
        public void has_the_name_from_the_file()
        {
            theHost.Name.ShouldEqual("h1");
        }
    }
}