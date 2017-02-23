using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authorization;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authorization
{
    
    public class SecuritySettingsTester
    {
        [Fact]
        public void authentication_is_enabled_by_default()
        {
            new SecuritySettings().AuthenticationEnabled
                .ShouldBeTrue();
        }

        [Fact]
        public void authorization_is_enabled_by_default()
        {
            new SecuritySettings().AuthorizationEnabled
                .ShouldBeTrue();
        }

        [Fact]
        public void reset_sets_everything_to_enabled()
        {
            var settings = new SecuritySettings
            {
                AuthenticationEnabled = false,
                AuthorizationEnabled = false
            };

            settings.Reset();
        }
    }
}