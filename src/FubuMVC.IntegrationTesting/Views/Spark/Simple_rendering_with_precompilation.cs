using FubuCore;
using FubuMVC.Core;
using FubuMVC.Spark;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class Simple_rendering_with_precompilation : ViewIntegrationContext
    {
        public Simple_rendering_with_precompilation()
        {
            SparkView<BreatheViewModel>("Breathe")
                .Write(@"
<p>This is real output</p>
<h2>${Model.Text}</h2>");
        }

        public class MyRegistry : FubuRegistry
        {
            public MyRegistry()
            {
                AlterSettings<SparkEngineSettings>(x => x.PrecompileViews = true);
            }
        }

        [Test]
        public void can_render()
        {
            Scenario.Get.Input(new AirInputModel{TakeABreath = true});
            Scenario.ContentShouldContain("<h2>Breathe in!</h2>");
        }
    }


}