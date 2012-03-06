using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests.RazorModel
{
    [TestFixture]
    public class TemplateRegistryTester : InteractionContext<TemplateRegistry<IRazorTemplate>>
    {
        private IList<IRazorTemplate> _templates;
        private IRazorTemplate[] _bindings;
        protected override void beforeEach()
        {
            _templates = new List<IRazorTemplate>
            {
                new Template("App/Shared/bindings.xml", "App", FubuRazorConstants.HostOrigin),
                new Template("App/bindings.xml", "App", FubuRazorConstants.HostOrigin),
                new Template("App/Views/binding.xml", "App", FubuRazorConstants.HostOrigin),
                new Template("App/Actions/binding.xml", "App", FubuRazorConstants.HostOrigin),
                new Template("App/Actions/Home/home.cshtml", "App", FubuRazorConstants.HostOrigin),
                new Template("App/Packages1/Views/Home/home.cshtml", "App/Package1", "Package1"),
                new Template("App/Packages1/Views/Products/list.cshtml", "App/Package1", "Package1"),
                new Template("App/Views/Home/home.cshtml", "App", FubuRazorConstants.HostOrigin)
            };


            var view = _templates.Last();
            view.ViewPath = view.FilePath;
            var descriptor = new ViewDescriptor(view);
            view.Descriptor = descriptor;

            Services.Inject(new TemplateRegistry<IRazorTemplate>(_templates));
        }

        [Test]
        public void first_by_name()
        {
            ClassUnderTest.FirstByName("home").ShouldNotBeNull().FilePath.ShouldEqual("App/Actions/Home/home.cshtml");
            ClassUnderTest.FirstByName("products").ShouldBeNull();
        }

        [Test]
        public void by_origin()
        {
            ClassUnderTest.ByOrigin("Package1").ShouldHaveCount(2)
                .All(x => x.Origin == "Package1").ShouldBeTrue();
            ClassUnderTest.ByOrigin(FubuRazorConstants.HostOrigin).ShouldHaveCount(6)
                .All(x => x.Origin == FubuRazorConstants.HostOrigin).ShouldBeTrue();
        }

        [Test]
        public void all_templates()
        {
            ClassUnderTest.AllTemplates().ShouldHaveCount(8).ShouldEqual(_templates);
        }

        [Test]
        public void from_host()
        {
            var fromHost = ClassUnderTest.FromHost();
            fromHost.ShouldHaveCount(6);
            fromHost.ShouldEqual(ClassUnderTest.ByOrigin(FubuRazorConstants.HostOrigin));
        }

    }
}