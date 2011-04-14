using FubuCore.Binding;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class SettingsProviderIntegratedTester
    {
        private FolderAppSettingsXmlSource theSource;
        private SettingsProvider theProvider;

        [SetUp]
        public void SetUp()
        {
            var resolver = ObjectResolver.Basic();

            theSource = new FolderAppSettingsXmlSource("Configuration");
            theProvider = new SettingsProvider(resolver, new ISettingsSource[]{theSource});
        }

        [Test]
        public void pull_a_settings_object_with_environment_overrides()
        {
            var settings = theProvider.SettingsFor<OneSettings>();
            settings.Name.ShouldEqual("Max");
            settings.Age.ShouldEqual(37);
        }

        [Test]
        public void pull_a_settings_object_without_environment_overrides()
        {
            var settings = theProvider.SettingsFor<ThreeSettings>();
            settings.Direction.ShouldEqual("North");
            settings.Threshold.ShouldEqual(3);
        }
    }
}