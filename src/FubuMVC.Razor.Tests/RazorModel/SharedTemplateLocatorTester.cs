using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.RazorModel
{
    [TestFixture]
    public class SharedTemplateLocatorTester : InteractionContext<SharedTemplateLocator>
    {
        private ITemplateDirectoryProvider _provider;
        private IRazorTemplate _template;
        private IList<string> _masterDirectories;
        private IList<string> _bindingDirectories;
        private TemplateRegistry<IRazorTemplate> _masterTemplateRegistry;
        private TemplateRegistry<IRazorTemplate> _bindingTemplateRegistry;

        protected override void beforeEach()
        {
            _template = MockFor<IRazorTemplate>();
            _masterDirectories = new List<string>
            {
                Path.Combine("App", "Actions", "Shared"),
                Path.Combine("App", "Views", "Shared"),
                Path.Combine("App", "Shared")
            };
            _bindingDirectories = new List<string>
            {
                Path.Combine("App", "Views", "Shared"),
                Path.Combine("App", "Views"),
                Path.Combine("App", "Shared"),
                Path.Combine("App")
            };

            _masterTemplateRegistry = new TemplateRegistry<IRazorTemplate>
            {
                new Template(Path.Combine("App", "Shared", "application.cshtml"), "App",TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Shared", "sitemaster.cshtml"), "App",TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "Shared", "application.cshtml"), "App",TemplateConstants.HostOrigin)
            };

            _bindingTemplateRegistry = new TemplateRegistry<IRazorTemplate>
            {
                new Template(Path.Combine("App", "bindings.xml"), "App",TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Shared", "application.cshtml"), "App",TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Shared", "bindings.xml"), "App",TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Shared", "sitemaster.cshtml"), "App",TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "bindings.xml"), "App",TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "Shared", "application.cshtml"), "App",TemplateConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "Shared", "bindings.xml"), "App",TemplateConstants.HostOrigin)
            };

            _provider = MockFor<ITemplateDirectoryProvider>();
            _provider.Stub(x => x.SharedPathsOf(_template, _masterTemplateRegistry)).Return(_masterDirectories);
            _provider.Stub(x => x.ReachablesOf(_template, _bindingTemplateRegistry)).Return(_bindingDirectories);
        }

        [Test]
        public void locate_master_returns_template_that_match_first_shared_directory_and_name()
        {
            var master = ClassUnderTest.LocateMaster("application", _template, _masterTemplateRegistry);
            master.ShouldNotBeNull().ShouldEqual(_masterTemplateRegistry[2]);
        }

        [Test]
        public void if_not_exists_locate_master_returns_null()
        {
            var master = ClassUnderTest.LocateMaster("admin", _template, _masterTemplateRegistry);
            master.ShouldBeNull();
        }
    }
}