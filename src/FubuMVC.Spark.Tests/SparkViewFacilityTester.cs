using System;
using System.IO;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class SparkViewFacilityTester : InteractionContext<SparkViewFacility>
    {
        private string _root;
        private TemplateRegistry<ITemplate> _templateRegistry;

        protected override void beforeEach()
        {
            _root = AppDomain.CurrentDomain.BaseDirectory;
            _templateRegistry = new TemplateRegistry<ITemplate>
            {
                new Template(Path.Combine(_root, "Views", "Home", "ModelAView.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial1.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "ModelBView.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial2.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "ModelCView.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial3.spark"), _root, TemplateConstants.HostOrigin)
            };
            
            _templateRegistry[0].Descriptor = new SparkDescriptor(_templateRegistry[0]) { ViewModel = typeof(ModelA) };
            _templateRegistry[2].Descriptor = new SparkDescriptor(_templateRegistry[2]) { ViewModel = typeof(ModelB) };
            _templateRegistry[4].Descriptor = new SparkDescriptor(_templateRegistry[4]) { ViewModel = typeof(ModelC) };

            Services.Inject<ITemplateRegistry<ITemplate>>(_templateRegistry);
        }

        public class ModelA { }
        public class ModelB { }
        public class ModelC { }

        [Test]
        public void find_views_returns_view_tokens_from_items_with_a_view_model_only()
        {
            var views = ClassUnderTest.FindViews(new TypePool(typeof(SparkViewFacilityTester).Assembly), new BehaviorGraph());
            views.ShouldHaveCount(3);
            views.ShouldContain(x => x.ViewModel == typeof(ModelA));
            views.ShouldContain(x => x.ViewModel == typeof(ModelB));
            views.ShouldContain(x => x.ViewModel == typeof(ModelC));
        }
    }
}