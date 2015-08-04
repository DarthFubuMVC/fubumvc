using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authorization;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authorization
{
    [TestFixture]
    public class SecuritySettingsTester
    {
        [Test]
        public void authentication_is_enabled_by_default()
        {
            new SecuritySettings().AuthenticationEnabled
                .ShouldBeTrue();
        }

        [Test]
        public void authorization_is_enabled_by_default()
        {
            new SecuritySettings().AuthorizationEnabled
                .ShouldBeTrue();
        }

        [Test]
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