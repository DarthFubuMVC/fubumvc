using FubuCore.Configuration;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.AppSettings
{
    [TestFixture]
    public class AppSettingsProviderIntegratedTester : InteractionContext<AppSettingsProvider>
    {
        protected override void beforeEach()
        {
            Container.Configure(x => x.AddRegistry<AppSettingProviderRegistry>());
        }

        [Test]
        public void fetch_a_simple_object()
        {
            // This data is pulled from the FubuMVC.Tests.dll.config file
            var settings = ClassUnderTest.SettingsFor<FakeSettings>();
            settings.Name.ShouldEqual("Max");
            settings.Age.ShouldEqual(6);
            settings.Active.ShouldBeTrue();
        }
    }
}