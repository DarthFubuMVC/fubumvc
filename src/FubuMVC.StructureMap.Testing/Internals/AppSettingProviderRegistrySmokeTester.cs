using FubuCore.Configuration;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.StructureMap.Testing.Internals
{
    [TestFixture]
    public class AppSettingProviderRegistrySmokeTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void can_build_an_app_settings_provider_object()
        {
            var container = new Container(new AppSettingProviderRegistry());
            container.GetInstance<AppSettingsProvider>().ShouldNotBeNull();
        }
    }
}