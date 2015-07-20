using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class RegisterAllSettingsTester
    {
        [Test]
        public void all_explicitly_changed_settings_should_get_registered_as_a_service()
        {
            var registry = new FubuRegistry();

            var oneSettings = new OneSettings();
            registry.ReplaceSettings(oneSettings);
            var twoSettings = new TwoSettings();
            registry.ReplaceSettings(twoSettings);
            var threeSettings = new ThreeSettings();
            registry.ReplaceSettings(threeSettings);

            using (var runtime = FubuApplication.For(registry).Bootstrap())
            {
                runtime.Container.DefaultRegistrationIs(oneSettings);
                runtime.Container.DefaultRegistrationIs(twoSettings);
                runtime.Container.DefaultRegistrationIs(threeSettings);
            }
        }
    }

    public class OneSettings
    {
    }

    public class TwoSettings
    {
    }

    public class ThreeSettings
    {
    }
}