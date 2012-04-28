using System;
using System.IO;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests
{
    [TestFixture]
    public class RazorViewFacilityTester : InteractionContext<RazorViewFacility>
    {
        private string _root;
        private TemplateRegistry<IRazorTemplate> _templateRegistry;

        protected override void beforeEach()
        {
            _root = AppDomain.CurrentDomain.BaseDirectory;
            _templateRegistry = new TemplateRegistry<IRazorTemplate>
            {
                new Template(Path.Combine(_root, "Views", "Home", "ModelAView.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial1.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "ModelBView.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial2.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "ModelCView.cshtml"), _root, TemplateConstants.HostOrigin),
                new Template(Path.Combine(_root, "Views", "Home", "_partial3.cshtml"), _root, TemplateConstants.HostOrigin)
            };
            
            _templateRegistry[0].Descriptor = new ViewDescriptor<IRazorTemplate>(_templateRegistry[0]) { ViewModel = typeof(ModelA) };
            _templateRegistry[2].Descriptor = new ViewDescriptor<IRazorTemplate>(_templateRegistry[2]) { ViewModel = typeof(ModelB) };
            _templateRegistry[4].Descriptor = new ViewDescriptor<IRazorTemplate>(_templateRegistry[4]) { ViewModel = typeof(ModelC) };

            Services.Inject<ITemplateRegistry<IRazorTemplate>>(_templateRegistry);
        }

        public class ModelA { }
        public class ModelB { }
        public class ModelC { }

        [Test]
        public void find_views_returns_view_tokens_from_items_with_a_view_model_only()
        {
            var views = ClassUnderTest.FindViews();
            views.ShouldHaveCount(3);
            views.ShouldContain(x => x.ViewModel == typeof(ModelA));
            views.ShouldContain(x => x.ViewModel == typeof(ModelB));
            views.ShouldContain(x => x.ViewModel == typeof(ModelC));
        }
    }
}