using System;
using System.IO;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class SparkViewFacilityTester : InteractionContext<SparkViewFacility>
    {
        private TemplateRegistry _templateRegistry;

        protected override void beforeEach()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            _templateRegistry = new TemplateRegistry
            {
                new Template(Path.Combine(root, "Views", "Home", "ModelAView.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "_partial1.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "ModelBView.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "_partial2.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "ModelCView.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "_partial3.spark"), root, FubuSparkConstants.HostOrigin)
            };
            
            _templateRegistry[0].Descriptor = new ViewDescriptor(_templateRegistry[0]) { ViewModel = typeof(ModelA) };
            _templateRegistry[2].Descriptor = new ViewDescriptor(_templateRegistry[2]) { ViewModel = typeof(ModelB) };
            _templateRegistry[4].Descriptor = new ViewDescriptor(_templateRegistry[4]) { ViewModel = typeof(ModelC) };

            Services.Inject<ITemplateRegistry>(_templateRegistry);
        }

        public class ModelA { }
        public class ModelB { }
        public class ModelC { }

        [Test]
        public void find_views_returns_view_tokens_from_items_with_a_view_model_only()
        {
            var views = ClassUnderTest.FindViews(new TypePool(typeof(SparkViewFacilityTester).Assembly), new BehaviorGraph());
            views.ShouldHaveCount(3);
            views.ShouldContain(x => x.ViewModelType == typeof(ModelA));
            views.ShouldContain(x => x.ViewModelType == typeof(ModelB));
            views.ShouldContain(x => x.ViewModelType == typeof(ModelC));
        }
    }
}