using Bottles.Configuration;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests
{
    [TestFixture]
    public class EnvironmentSettingsTester
    {
        private EnvironmentSettings theEnvironmentSettings;

        [SetUp]
        public void SetUp()
        {
            theEnvironmentSettings = new EnvironmentSettings();
        }

        [Test]
        public void read_text_with_no_equals()
        {
            Exception<EnvironmentSettingsException>.ShouldBeThrownBy(() =>
            {
                theEnvironmentSettings.ReadText("something");
            });
            
        }

        [Test]
        public void read_text_with_too_many_equals()
        {
            Exception<EnvironmentSettingsException>.ShouldBeThrownBy(() =>
            {
                theEnvironmentSettings.ReadText("something=else=more");
            });

        }

        [Test]
        public void read_text_with_equals_and_only_one_dot()
        {
            theEnvironmentSettings.ReadText("arg.1=value");
            theEnvironmentSettings.EnvironmentSettingsData()["arg.1"].ShouldEqual("value");
        }

        [Test]
        public void environment_settings_must_be_categorized_as_environment()
        {
            theEnvironmentSettings.EnvironmentSettingsData().Category.ShouldEqual(SettingCategory.environment);
        }

        [Test]
        public void read_simple_value()
        {
            theEnvironmentSettings.ReadText("A=B");
            theEnvironmentSettings.ReadText("C=D");

            theEnvironmentSettings.Overrides["A"].ShouldEqual("B");
            theEnvironmentSettings.Overrides["C"].ShouldEqual("D");
        }

        [Test]
        public void read_host_directive()
        {
            theEnvironmentSettings.ReadText("Host1.OneDirective.Name=Jeremy");
            theEnvironmentSettings.ReadText("Host1.OneDirective.Age=45");
            theEnvironmentSettings.ReadText("Host2.OneDirective.Name=Tom");

            theEnvironmentSettings.DataForHost("Host1").Get("OneDirective.Name").ShouldEqual("Jeremy");
            theEnvironmentSettings.DataForHost("Host2").Get("OneDirective.Name").ShouldEqual("Tom");
            theEnvironmentSettings.DataForHost("Host1").Get("OneDirective.Age").ShouldEqual("45");
        }

        [Test]
        public void the_environment_settings_are_categorized_as_environment()
        {
            theEnvironmentSettings.DataForHost("Host1").Category.ShouldEqual(SettingCategory.environment);
            theEnvironmentSettings.DataForHost("Host2").Category.ShouldEqual(SettingCategory.environment);
        }
    }
}