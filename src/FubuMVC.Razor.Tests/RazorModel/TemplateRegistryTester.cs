using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
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
        protected override void beforeEach()
        {
            _templates = new List<IRazorTemplate>
            {
                new RazorTemplate("App/Shared/bindings.xml", "App", ContentFolder.Application),
                new RazorTemplate("App/bindings.xml", "App", ContentFolder.Application),
                new RazorTemplate("App/Views/binding.xml", "App", ContentFolder.Application),
                new RazorTemplate("App/Actions/binding.xml", "App", ContentFolder.Application),
                new RazorTemplate("App/Actions/Home/home.cshtml", "App", ContentFolder.Application),
                new RazorTemplate("App/Packages1/Views/Home/home.cshtml", "App/Package1", "Package1"),
                new RazorTemplate("App/Packages1/Views/Products/list.cshtml", "App/Package1", "Package1"),
                new RazorTemplate("App/Views/Home/home.cshtml", "App", ContentFolder.Application)
            };


            var view = _templates.Last();
            view.As<RazorTemplate>().ViewPath = view.FilePath;
            var descriptor = new ViewDescriptor<IRazorTemplate>(view);
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
            ClassUnderTest.ByOrigin(ContentFolder.Application).ShouldHaveCount(6)
                .All(x => x.Origin == ContentFolder.Application).ShouldBeTrue();
        }

        [Test]
        public void all_templates()
        {
            ClassUnderTest.ShouldHaveCount(8).ShouldEqual(_templates);
        }

        [Test]
        public void from_host()
        {
            var fromHost = ClassUnderTest.FromHost();
            fromHost.ShouldHaveCount(6);
            fromHost.ShouldEqual(ClassUnderTest.ByOrigin(ContentFolder.Application));
        }

    }
}