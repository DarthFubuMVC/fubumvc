using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class FubuBindingProviderTester : InteractionContext<FubuBindingProvider>
    {
        private  ISparkTemplates _templates;

        protected override void beforeEach()
        {
            _templates = MockFor<ISparkTemplates>();
        }

        [Test]
        public void smoke()
        {
        }

    }
}