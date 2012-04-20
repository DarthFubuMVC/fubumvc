using FubuMVC.Core;
using FubuMVC.Spark;
using FubuTestingSupport;
using IntegrationTesting.Conneg;
using NUnit.Framework;

namespace IntegrationTesting.ViewEngines.HelloSpark
{
    [TestFixture]
    public class SparkEngineIntegrationTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<AirController>();

            registry.IncludeDiagnostics(true);

            registry.UseSpark();

            registry.Views
                .TryToAttachViewsInPackages()
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

        //[Test]
        //public void what_is_there()
        //{
        //    var text = endpoints.Get<BehaviorGraphWriter>(x => x.PrintRoutes()).ReadAsText();

        //    Debug.WriteLine(text);
        //}
    }
}