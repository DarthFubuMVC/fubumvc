using System;
using System.IO;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests
{
    // TODO: Add more coverage

    [TestFixture]
    public class RazorViewFacilityTester : InteractionContext<RazorViewFacility>
    {
        private string _root;
        private TemplateRegistry<IRazorTemplate> _templateRegistry;

        protected override void beforeEach()
        {
            _root = AppDomain.CurrentDomain.BaseDirectory;

            _templateRegistry = new TemplateRegistry<IRazorTemplate>(new[]
            {
                new Template(Path.Combine(_root, "Views", "Home", "ModelAView.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial1.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "ModelBView.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial2.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "ModelCView.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial3.cshtml"), _root, TemplateConstants.HostOrigin)
            });

            var templates = _templateRegistry.ToList();

            templates[0].Descriptor = new ViewDescriptor<IRazorTemplate>(templates[0]) { ViewModel = typeof(ModelA) };
            templates[2].Descriptor = new ViewDescriptor<IRazorTemplate>(templates[2]) { ViewModel = typeof(ModelB) };
            templates[4].Descriptor = new ViewDescriptor<IRazorTemplate>(templates[4]) { ViewModel = typeof(ModelC) };

            Services.Inject(_templateRegistry);
        }

        public class ModelA { }
        public class ModelB { }
        public class ModelC { }

        [Test]
        public void find_views_returns_view_tokens_from_items_with_a_view_model_only()
        {
            // If this tests breaks, look at pathing issues FIRST
            var views = ClassUnderTest.FindViews(new BehaviorGraph()).Result.ToList();

            views.ShouldHaveCount(3);
            views.ShouldContain(x => x.ViewModel == typeof(ModelA));
            views.ShouldContain(x => x.ViewModel == typeof(ModelB));
            views.ShouldContain(x => x.ViewModel == typeof(ModelC));
        }
    }
}