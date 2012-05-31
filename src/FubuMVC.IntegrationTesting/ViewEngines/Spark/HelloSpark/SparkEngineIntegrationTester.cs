using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.Spark;
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

            registry.IncludeDiagnostics(true);

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

        [Test]
        public void what_is_there()
        {
            var text = endpoints.Get<BehaviorGraphWriter>(x => x.PrintRoutes()).ReadAsText();

            Debug.WriteLine(text);
        }
    }




}