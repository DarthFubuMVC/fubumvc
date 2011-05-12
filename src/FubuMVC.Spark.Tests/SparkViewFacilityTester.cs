using System;
using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class SparkViewFacilityTester : InteractionContext<SparkViewFacility>
    {
        private IList<ITemplate> _templates;

        protected override void beforeEach()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            _templates = new List<ITemplate>
            {
                new Template(Path.Combine(root, "Views", "Home", "ModelAView.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "_partial1.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "ModelBView.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "_partial2.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "ModelCView.spark"), root, FubuSparkConstants.HostOrigin),
                new Template(Path.Combine(root, "Views", "Home", "_partial3.spark"), root, FubuSparkConstants.HostOrigin)
            };
            
            _templates[0].Descriptor = new ViewDescriptor(_templates[0]) { ViewModel = typeof(ModelA) };
            _templates[2].Descriptor = new ViewDescriptor(_templates[2]) { ViewModel = typeof(ModelB) };
            _templates[4].Descriptor = new ViewDescriptor(_templates[4]) { ViewModel = typeof(ModelC) };
            
            MockFor<ITemplateComposer>().Expect(c => c.Compose(Arg<TypePool>.Is.Anything)).Return(_templates);
        }

        public class ModelA { }
        public class ModelB { }
        public class ModelC { }

        [Test]
        public void find_views_returns_view_tokens_from_items_with_a_view_model_only()
        {
            var views = ClassUnderTest.FindViews(null, null);
            views.ShouldHaveCount(3);
            views.ShouldContain(x => x.ViewModelType == typeof(ModelA));
            views.ShouldContain(x => x.ViewModelType == typeof(ModelB));
            views.ShouldContain(x => x.ViewModelType == typeof(ModelC));
        }

    }
}