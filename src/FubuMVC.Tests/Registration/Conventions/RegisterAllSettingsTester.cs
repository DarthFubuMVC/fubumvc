using FubuMVC.Core;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuTestingSupport;

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

            var graph = BehaviorGraph.BuildFrom(registry);

            graph.Services.DefaultServiceFor<OneSettings>().Value.ShouldBeTheSameAs(oneSettings);
            graph.Services.DefaultServiceFor<TwoSettings>().Value.ShouldBeTheSameAs(twoSettings);
            graph.Services.DefaultServiceFor<ThreeSettings>().Value.ShouldBeTheSameAs(threeSettings);
        }
    }

    public class OneSettings{}
    public class TwoSettings{}
    public class ThreeSettings{}
}