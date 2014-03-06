using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class shared_template_locator_master_tester : InteractionContext<SharedTemplateLocator>
    {
        private ISparkTemplate _template;
        private IList<string> _directories;
        private TemplateRegistry<ISparkTemplate> _templates;

        protected override void beforeEach()
        {
            _template = MockFor<ISparkTemplate>();
            _directories = new List<string>
            {
                Path.Combine("App", "Actions", "Shared"),
                Path.Combine("App", "Views", "Shared"),
                Path.Combine("App", "Shared")
            };

            _templates = new TemplateRegistry<ISparkTemplate>(new[]
            {
                new SparkTemplate(Path.Combine("App", "Shared", "application.spark"), "App", ContentFolder.Application),
                new SparkTemplate(Path.Combine("App", "Shared", "sitemaster.spark"), "App", ContentFolder.Application),
                new SparkTemplate(Path.Combine("App", "Views", "Shared", "application.spark"), "App", ContentFolder.Application),
                new SparkTemplate(Path.Combine("App", "Views", "Shared", "site.xml"), "App", ContentFolder.Application)
            });

            Container.Inject<ITemplateSelector<ISparkTemplate>>(new SparkTemplateSelector());

            MockFor<ITemplateDirectoryProvider<ISparkTemplate>>()
                .Stub(x => x.SharedPathsOf(_template)).Return(_directories);

            Container.Inject<ITemplateRegistry<ISparkTemplate>>(_templates);
        }

        [Test]
        public void locate_master_returns_template_that_match_first_shared_directory_and_name()
        {
            ClassUnderTest.LocateMaster("application", _template)
                .ShouldNotBeNull()
                .ShouldEqual(_templates.ElementAt(2));
        }

        [Test]
        public void if_not_exists_locate_master_returns_null()
        {
            ClassUnderTest.LocateMaster("admin", _template)
                .ShouldBeNull();
        }

        [Test]
        public void locate_master_only_considers_spark_views()
        {
            ClassUnderTest.LocateMaster("site", _template)
                .ShouldBeNull();
        }
    }

    public class shared_template_locator_bindings_tester : InteractionContext<SharedTemplateLocator>
    {
        private TemplateRegistry<ISparkTemplate> _templates;
        private IList<string> _bindingDirectories;
        private ISparkTemplate _template;

        protected override void beforeEach()
        {
            _template = MockFor<ISparkTemplate>();
            _bindingDirectories = new List<string>
            {
                Path.Combine("App", "Views", "Shared"),
                Path.Combine("App", "Views"),
                Path.Combine("App", "Shared"),
                "App"
            };

            _templates = new TemplateRegistry<ISparkTemplate>(new[]
            {
                new SparkTemplate(Path.Combine("App", "bindings.xml"), "App",ContentFolder.Application),
                new SparkTemplate(Path.Combine("App", "Shared", "application.spark"), "App",ContentFolder.Application),
                new SparkTemplate(Path.Combine("App", "Shared", "bindings.xml"), "App",ContentFolder.Application),
                new SparkTemplate(Path.Combine("App", "Shared", "sitemaster.spark"), "App",ContentFolder.Application),
                new SparkTemplate(Path.Combine("App", "Views", "bindings.xml"), "App",ContentFolder.Application),
                new SparkTemplate(Path.Combine("App", "Views", "Shared", "application.spark"), "App",ContentFolder.Application),
                new SparkTemplate(Path.Combine("App", "Views", "Shared", "bindings.xml"), "App",ContentFolder.Application)
            });

            MockFor<ITemplateDirectoryProvider<ISparkTemplate>>()
                .Stub(x => x.ReachablesOf(_template))
                .Return(_bindingDirectories);

            Container.Inject<ITemplateRegistry<ISparkTemplate>>(_templates);
        }

        [Test]
        public void locate_binding_returns_template_that_match_shared_directories_and_name()
        {
            var bindings = ClassUnderTest.LocateBindings("bindings", _template).ToList();
            bindings.ShouldNotBeNull().ShouldHaveCount(4);

            bindings.ElementAt(0).ShouldEqual(_templates.ElementAt(6));
            bindings.ElementAt(1).ShouldEqual(_templates.ElementAt(4));
            bindings.ElementAt(2).ShouldEqual(_templates.ElementAt(2));
            bindings.ElementAt(3).ShouldEqual(_templates.ElementAt(0));
        }

        [Test]
        public void locate_bindings_returns_empty_list_if_none_exist()
        {
            ClassUnderTest.LocateBindings("sparkbindings", _template)
                .ShouldNotBeNull()
                .ShouldHaveCount(0);
        }      
  
        [Test]
        public void locate_bindings_only_considers_xml_files()
        {
            ClassUnderTest.LocateBindings("application", _template)
                .ShouldHaveCount(0);
        }
    }
}