using FubuMVC.Core.Configuration;
using FubuMVC.StructureMap;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.StructureMapIoC
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