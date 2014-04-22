using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class Application_settings_alteration_wins_out_over_an_import
    {
        [Test]
        public void application_settings_wins()
        {
            var import = new FubuRegistry();
            import.AlterSettings<FireflySettings>(o=> o.HowManyHaveYouCaught = 5);

            var graph = BehaviorGraph.BuildFrom(x => {
                x.AlterSettings<FireflySettings>(o => o.HowManyHaveYouCaught =11);

                x.Import(import, string.Empty);
            });

            graph.Settings.Get<FireflySettings>()
                .HowManyHaveYouCaught.ShouldEqual(11);
        }
    }

    public class FireflySettings
    {
        public int HowManyHaveYouCaught;
    }

}