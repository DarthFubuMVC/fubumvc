using System;
using System.IO;
using System.Linq;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests
{
     // TODO: More UT

    [TestFixture]
    public class SparkViewFacilityTester : InteractionContext<SparkViewFacility>
    {
        private string _root;
        private SparkTemplateRegistry _templateRegistry;

        protected override void beforeEach()
        {
            _root = AppDomain.CurrentDomain.BaseDirectory;
            _templateRegistry = new SparkTemplateRegistry(new[]
            {
                new Template(Path.Combine(_root, "Views", "Home", "ModelAView.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial1.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "ModelBView.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial2.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "ModelCView.spark"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial3.spark"), _root, TemplateConstants.HostOrigin)
            });

            var templates = _templateRegistry.ToList();
            templates[0].Descriptor = new SparkDescriptor(templates[0]) { ViewModel = typeof(ModelA) };
            templates[2].Descriptor = new SparkDescriptor(templates[2]) { ViewModel = typeof(ModelB) };
            templates[4].Descriptor = new SparkDescriptor(templates[4]) { ViewModel = typeof(ModelC) };

            Services.Inject(_templateRegistry);
        }

        public class ModelA { }
        public class ModelB { }
        public class ModelC { }

        [Test]
        public void find_views_returns_view_tokens_from_items_with_a_view_model_only()
        {
            var views = ClassUnderTest.FindTokens().ToList();

            views.ShouldHaveCount(3);
            views.ShouldContain(x => x.ViewModel == typeof(ModelA));
            views.ShouldContain(x => x.ViewModel == typeof(ModelB));
            views.ShouldContain(x => x.ViewModel == typeof(ModelC));
        }
    }
}