using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    //[TestFixture]
    //public class SharedTemplateLocatorTester : InteractionContext<SharedTemplateLocator>
    //{
    //    private ITemplateDirectoryProvider _provider;
    //    private ITemplate _template;
    //    private IList<string> _masterDirectories;
    //    private IList<string> _bindingDirectories;
    //    private TemplateRegistry _masterTemplateRegistry;
    //    private TemplateRegistry _bindingTemplateRegistry;

    //    public class MyClass
    //    {
             
    //    }

    //    protected override void beforeEach()
    //    {
    //        _template = MockFor<ITemplate>();
    //        _masterDirectories = new List<string>
    //        {
    //            Path.Combine("App", "Actions", "Shared"),
    //            Path.Combine("App", "Views", "Shared"),
    //            Path.Combine("App", "Shared")
    //        };
    //        _bindingDirectories = new List<string>
    //        {
    //            Path.Combine("App", "Views", "Shared"),
    //            Path.Combine("App", "Views"),
    //            Path.Combine("App", "Shared"),
    //            Path.Combine("App")
    //        };

    //        _masterTemplateRegistry = new TemplateRegistry
    //        {
    //            new Template(Path.Combine("App", "Shared", "application.spark"), "App",FubuSparkConstants.HostOrigin),
    //            new Template(Path.Combine("App", "Shared", "sitemaster.spark"), "App",FubuSparkConstants.HostOrigin),
    //            new Template(Path.Combine("App", "Views", "Shared", "application.spark"), "App",FubuSparkConstants.HostOrigin)
    //        };

    //        _bindingTemplateRegistry = new TemplateRegistry
    //        {
    //            new Template(Path.Combine("App", "bindings.xml"), "App",FubuSparkConstants.HostOrigin),
    //            new Template(Path.Combine("App", "Shared", "application.spark"), "App",FubuSparkConstants.HostOrigin),
    //            new Template(Path.Combine("App", "Shared", "bindings.xml"), "App",FubuSparkConstants.HostOrigin),
    //            new Template(Path.Combine("App", "Shared", "sitemaster.spark"), "App",FubuSparkConstants.HostOrigin),
    //            new Template(Path.Combine("App", "Views", "bindings.xml"), "App",FubuSparkConstants.HostOrigin),
    //            new Template(Path.Combine("App", "Views", "Shared", "application.spark"), "App",FubuSparkConstants.HostOrigin),
    //            new Template(Path.Combine("App", "Views", "Shared", "bindings.xml"), "App",FubuSparkConstants.HostOrigin)
    //        };

    //        _provider = MockFor<ITemplateDirectoryProvider>();
    //        _provider.Stub(x => x.SharedPathsOf(_template, _masterTemplateRegistry)).Return(_masterDirectories);
    //        _provider.Stub(x => x.ReachablesOf(_template, _bindingTemplateRegistry)).Return(_bindingDirectories);
    //    }

    //    [Test]
    //    public void locate_master_returns_template_that_match_first_shared_directory_and_name()
    //    {
    //        var master = ClassUnderTest.LocateMaster("application", _template, _masterTemplateRegistry);
    //        master.ShouldNotBeNull().ShouldEqual(_masterTemplateRegistry[2]);
    //    }

    //    [Test]
    //    public void if_not_exists_locate_master_returns_null()
    //    {
    //        var master = ClassUnderTest.LocateMaster("admin", _template, _masterTemplateRegistry);
    //        master.ShouldBeNull();
    //    }

    //    [Test]
    //    public void locate_binding_returns_template_that_match_shared_directories_and_name()
    //    {
    //        var bindings = ClassUnderTest.LocateBindings("bindings", _template, _bindingTemplateRegistry);
    //        bindings.ShouldNotBeNull().ShouldHaveCount(4);

    //        bindings.ElementAt(0).ShouldEqual(_bindingTemplateRegistry[6]);
    //        bindings.ElementAt(1).ShouldEqual(_bindingTemplateRegistry[4]);
    //        bindings.ElementAt(2).ShouldEqual(_bindingTemplateRegistry[2]);
    //        bindings.ElementAt(3).ShouldEqual(_bindingTemplateRegistry[0]);
    //    }

    //    [Test]
    //    public void if_not_exists_locate_bindings_returns_empty_list()
    //    {
    //        var bindings = ClassUnderTest.LocateBindings("sparkbindings", _template, _bindingTemplateRegistry);
    //        bindings.ShouldNotBeNull().ShouldHaveCount(0);
    //    }

    //}
}