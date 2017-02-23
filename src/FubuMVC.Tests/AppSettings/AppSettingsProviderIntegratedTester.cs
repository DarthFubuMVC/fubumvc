using FubuCore.Binding;
using FubuCore.Configuration;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.AppSettings
{
    
    public class AppSettingsProviderIntegratedTester 
    {

        [Fact]
        public void fetch_a_simple_object()
        {
            var provider = new AppSettingsProvider(ObjectResolver.Basic());

            // This data is pulled from the FubuMVC.Tests.dll.config file
            var settings = provider.SettingsFor<FakeSettings>();
            settings.Name.ShouldBe("Max");
            settings.Age.ShouldBe(6);
            settings.Active.ShouldBeTrue();
        }
    }
}