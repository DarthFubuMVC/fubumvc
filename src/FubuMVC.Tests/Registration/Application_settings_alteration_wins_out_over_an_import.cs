using FubuMVC.Core;
using FubuMVC.Core.Registration;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Registration
{
    
    public class Application_settings_alteration_wins_out_over_an_import
    {
        [Fact]
        public void application_settings_wins()
        {
            var import = new FubuRegistry();
            import.AlterSettings<FireflySettings>(o=> o.HowManyHaveYouCaught = 5);

            var graph = BehaviorGraph.BuildFrom(x => {
                x.AlterSettings<FireflySettings>(o => o.HowManyHaveYouCaught =11);

                x.Import(import, string.Empty);
            });

            graph.Settings.Get<FireflySettings>()
                .HowManyHaveYouCaught.ShouldBe(11);
        }
    }

    public class FireflySettings
    {
        public int HowManyHaveYouCaught;
    }

}