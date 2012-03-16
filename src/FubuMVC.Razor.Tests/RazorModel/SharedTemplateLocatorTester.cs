using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.RazorModel
{
    [TestFixture]
    public class SharedTemplateLocatorTester : InteractionContext<SharedTemplateLocator<IRazorTemplate>>
    {
        private IRazorTemplate _template;
        private IList<string> _directories;
        private TemplateRegistry<IRazorTemplate> _templates;

        protected override void beforeEach()
        {
            _template = MockFor<IRazorTemplate>();
            _directories = new List<string>
            {
                Path.Combine("App", "Actions", "Shared"),
                Path.Combine("App", "Views", "Shared"),
                Path.Combine("App", "Shared")
            };

            _templates = new TemplateRegistry<IRazorTemplate>
            {
                new Template(Path.Combine("App", "Shared", "application.cshtml"), "App", TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Shared", "sitemaster.cshtml"), "App", TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "Shared", "application.cshtml"), "App", TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "Shared", "site.xml"), "App", TemplateConstants.HostOrigin)
            };

            MockFor<ITemplateDirectoryProvider<IRazorTemplate>>()
                .Stub(x => x.SharedPathsOf(_template)).Return(_directories);

            Container.Inject<ITemplateRegistry<IRazorTemplate>>(_templates);
        }

        [Test]
        public void locate_master_returns_template_that_match_first_shared_directory_and_name()
        {
            ClassUnderTest.LocateMaster("application", _template)
                .ShouldNotBeNull()
                .ShouldEqual(_templates[2]);
        }

        [Test]
        public void if_not_exists_locate_master_returns_null()
        {
            ClassUnderTest.LocateMaster("admin", _template)
                .ShouldBeNull();
        }
    }
}