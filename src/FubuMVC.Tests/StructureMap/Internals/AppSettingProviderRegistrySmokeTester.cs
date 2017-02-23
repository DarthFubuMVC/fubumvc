using FubuCore.Configuration;
using FubuCore.Conversion;
using FubuMVC.Core.StructureMap;
using Shouldly;
using Xunit;
using StructureMap;

namespace FubuMVC.Tests.StructureMap.Internals
{
    
    public class AppSettingProviderRegistrySmokeTester
    {
        [Fact]
        public void can_build_an_app_settings_provider_object()
        {
            var container = new Container(new AppSettingProviderRegistry());
            container.GetInstance<AppSettingsProvider>().ShouldNotBeNull();
        }

        [Fact]
        public void can_build_the_object_converter()
        {
            var container = new Container(new AppSettingProviderRegistry());
            container.GetInstance<IObjectConverter>().ShouldNotBeNull();
        }
    }
}