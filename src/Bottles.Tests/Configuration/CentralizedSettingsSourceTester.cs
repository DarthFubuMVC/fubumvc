using Bottles.Configuration;
using Bottles.Tests.Deployment.Parsing;
using FubuCore.Binding;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace Bottles.Tests.Configuration
{
    [TestFixture]
    public class CentralizedSettingsSourceTester
    {
        private CentralizedSettingsSource theSource;
        private SettingsProvider theProvider;

        [SetUp]
        public void SetUp()
        {
            theSource = new CentralizedSettingsSource("Configuration");

            theProvider = new SettingsProvider(ObjectResolver.Basic(), theSource.FindSettingData());
        }

        [Test]
        public void used_environment_substitutions()
        {
            theProvider.SettingsFor<OneSettings>().Age.ShouldEqual(111); // this value is overriden in the environment.settings file
        }

        [Test]
        public void environment_property_wins()
        {
            theProvider.SettingsFor<OneSettings>().Name.ShouldEqual("Ivy");
        }

        [Test]
        public void other_files_are_picked_up()
        {
            theProvider.SettingsFor<TwoSettings>().City.ShouldEqual("Austin");
        }
    }
}