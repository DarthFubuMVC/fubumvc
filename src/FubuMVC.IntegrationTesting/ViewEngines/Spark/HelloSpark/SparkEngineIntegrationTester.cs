using FubuMVC.Core;
using FubuMVC.IntegrationTesting.Conneg;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.HelloSpark
{
    [TestFixture]
    public class SparkEngineIntegrationTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<AirController>();

            registry.Views
                .TryToAttachWithDefaultConventions();
        }

        [Test]
        public void simple_view()
        {
            var text = endpoints.Get<AirController>(x => x.Breathe(new AirInputModel{
                TakeABreath = true
            })).ReadAsText();

            text.ShouldContain("<h2>Exhale!</h2>");
        }
    }
}