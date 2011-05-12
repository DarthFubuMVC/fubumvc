using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class TemplateLocatorTester : InteractionContext<TemplateLocator>
    {
        private ITemplateDirectoryProvider _provider;
        private ITemplate _template;
        private IList<string> _masterDirectories;
        private IList<string> _bindingDirectories;
        private IList<ITemplate> _masterTemplates;
        private IList<ITemplate> _bindingTemplates;

        protected override void beforeEach()
        {
            _template = MockFor<ITemplate>();
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
         
            _masterTemplates = new List<ITemplate>
            {
                new Template(Path.Combine("App", "Shared", "application.spark"), "App",FubuSparkConstants.HostOrigin),
                new Template(Path.Combine("App", "Shared", "sitemaster.spark"), "App",FubuSparkConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "Shared", "application.spark"), "App",FubuSparkConstants.HostOrigin)
            };

            _bindingTemplates = new List<ITemplate>
            {
                new Template(Path.Combine("App", "bindings.xml"), "App",FubuSparkConstants.HostOrigin),
                new Template(Path.Combine("App", "Shared", "application.spark"), "App",FubuSparkConstants.HostOrigin),
                new Template(Path.Combine("App", "Shared", "bindings.xml"), "App",FubuSparkConstants.HostOrigin),
                new Template(Path.Combine("App", "Shared", "sitemaster.spark"), "App",FubuSparkConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "bindings.xml"), "App",FubuSparkConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "Shared", "application.spark"), "App",FubuSparkConstants.HostOrigin),
                new Template(Path.Combine("App", "Views", "Shared", "bindings.xml"), "App",FubuSparkConstants.HostOrigin)
            };

            _provider = MockFor<ITemplateDirectoryProvider>();
            _provider.Stub(x => x.SharedPathsOf(_template, _masterTemplates)).Return(_masterDirectories);
            _provider.Stub(x => x.ReachablesOf(_template, _bindingTemplates)).Return(_bindingDirectories);
        }

        [Test]
        public void locate_master_returns_template_that_match_first_shared_directory_and_name()
        {
            var master = ClassUnderTest.LocateMaster("application", _template, _masterTemplates);
            master.ShouldNotBeNull().ShouldEqual(_masterTemplates[2]);
        }

        [Test]
        public void if_not_exists_locate_master_returns_null()
        {
            var master = ClassUnderTest.LocateMaster("admin", _template, _masterTemplates);
            master.ShouldBeNull();
        }

        [Test]
        public void locate_binding_returns_template_that_match_shared_directories_and_name()
        {
            var bindings = ClassUnderTest.LocateBindings("bindings", _template, _bindingTemplates);
            bindings.ShouldNotBeNull().ShouldHaveCount(4);

            bindings.ElementAt(0).ShouldEqual(_bindingTemplates[6]);
            bindings.ElementAt(1).ShouldEqual(_bindingTemplates[4]);
            bindings.ElementAt(2).ShouldEqual(_bindingTemplates[2]);
            bindings.ElementAt(3).ShouldEqual(_bindingTemplates[0]);
        }

        [Test]
        public void if_not_exists_locate_bindings_returns_empty_list()
        {
            var bindings = ClassUnderTest.LocateBindings("sparkbindings", _template, _bindingTemplates);
            bindings.ShouldNotBeNull().ShouldHaveCount(0);
        }

    }
}