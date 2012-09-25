using FubuMVC.Core;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.HelloSpark
{
    [TestFixture]
    public class SparkEngineIntegrationTester : SharedHarnessContext
    {
        [Test]
        public void simple_view()
        {
            var text = endpoints.Get<AirEndpoint>(x => x.Breathe(new AirInputModel{
                TakeABreath = true
            })).ReadAsText();

            text.ShouldContain("<h2>Exhale!</h2>");
        }
    }
}