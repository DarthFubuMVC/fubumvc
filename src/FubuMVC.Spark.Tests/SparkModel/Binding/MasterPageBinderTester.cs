using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
    // TODO: FIX UP THIS MESS

    //[TestFixture]
    //public class MasterPageBinderTester : InteractionContext<MasterAttacher>
    //{
    //    private AttachRequest _request;
    //    private Parsing _parsing;
    //    private TemplateRegistry _templateRegistry;

    //    const string Host = FubuSparkConstants.HostOrigin;
    //    const string Pak1 = "pak1";
    //    const string Pak2 = "pak2";
    //    const string Pak3 = "pak3";

    //    private readonly string _hostRoot;
    //    private readonly string _pak1Root;
    //    private readonly string _pak2Root;
    //    private readonly string _pak3Root;

    //    public MasterPageBinderTester()
    //    {
    //        _hostRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inetpub", "www", "web");
    //        _pak1Root = Path.Combine(_hostRoot, Pak1);
    //        _pak2Root = Path.Combine(_hostRoot, Pak2);
    //        _pak3Root = Path.Combine(_hostRoot, Pak3);
    //    }

    //    protected override void beforeEach()
    //    {
    //        _templateRegistry = createTemplates();
    //        Services.Inject<ISharedTemplateLocator>(new SharedTemplateLocator(new TemplateDirectoryProvider(), ));
            
    //        _parsing = new Parsing
    //        {
    //            Master = "application",
    //            ViewModelType = typeof(ProductModel).FullName                               
    //        };

    //        MockFor<IParsingRegistrations>().Expect(x => x.ParsingFor(Arg<ITemplate>.Is.Anything)).Return(_parsing);

    //        _request = new AttachRequest
    //        {
    //            Logger = MockFor<ISparkLogger>()
    //        };
    //    }

    //    private TemplateRegistry createTemplates()
    //    {
    //        return new TemplateRegistry
    //        {
    //            newTemplate(_pak1Root, Pak1, true, "Actions", "Controllers", "Home", "Home.spark"), // 0
    //            newTemplate(_pak1Root, Pak1, true, "Actions", "Handlers", "Products", "list.spark"), // 1
    //            newTemplate(_pak1Root, Pak1, false, "Actions", "Shared", "application.spark"), // 2
    //            newTemplate(_pak2Root, Pak2, true, "Features", "Controllers", "Home", "Home.spark"), // 3
    //            newTemplate(_pak2Root, Pak2, true, "Features", "Handlers", "Products", "list.spark"), // 4
    //            newTemplate(_pak2Root, Pak2, false, "Shared", "application.spark"), // 5
                
    //            newTemplate(_pak3Root, Pak3, true, "Features", "Controllers", "Home", "Home.spark"), // 6
				
    //            newTemplate(_hostRoot, Host, false, "Actions", "Shared", "application.spark"), // 7
    //            newTemplate(_hostRoot, Host, true, "Features", "Mixer", "chuck.spark"), // 8
    //            newTemplate(_hostRoot, Host, false, "Features", "Mixer", "Shared", "application.spark"), // 9
    //            newTemplate(_hostRoot, Host, true, "Features", "roundkick.spark"), // 10
    //            newTemplate(_hostRoot, Host, true, "Handlers", "Products", "details.spark"), // 11
    //            newTemplate(_hostRoot, Host, false, "Shared", "bindings.xml"), // 12
    //            newTemplate(_hostRoot, Host, false, "Shared", "_Partial.spark"), // 13
    //            newTemplate(_hostRoot, Host, false, "Shared", "application.spark") // 14
    //        };
    //    }

    //    private static ITemplate newTemplate(string root, string origin, bool isView, params string[] relativePaths)
    //    {
    //        var paths = new[] { root }.Union(relativePaths).ToArray();
    //        var template = new Template(FileSystem.Combine(paths), root, origin);
    //        if (isView)
    //        {
    //            template.Descriptor = new ViewDescriptor(template);
    //        }
    //        return template;
    //    }

    //    [Test]
    //    public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_1()
    //    {
    //        var template = _templateRegistry.First();
    //        _request.Template = template;

    //        ClassUnderTest.Attach(_request);
    //        _templateRegistry.ElementAt(2).ShouldEqual(template.Descriptor.As<ViewDescriptor>().Master);
    //    }

    //    [Test]
    //    public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_2()
    //    {
    //        var template = _templateRegistry.ElementAt(3);
    //        _request.Template = template;

    //        ClassUnderTest.Attach(_request);
    //        template.Descriptor.As<ViewDescriptor>().Master.ShouldEqual(_templateRegistry.ElementAt(5));
    //    }

    //    [Test]
    //    public void fallback_to_master_in_shared_host_when_no_local_ancestor_exists()
    //    {
    //        var template = _templateRegistry.ElementAt(6);
    //        _request.Template = template;

    //        ClassUnderTest.Attach(_request);
    //        template.Descriptor.As<ViewDescriptor>().Master.ShouldEqual(_templateRegistry.Last());
    //    }

    //    [Test]
    //    public void fallback_to_master_in_host_1()
    //    {
    //        var template = _templateRegistry.ElementAt(8);
    //        _request.Template = template;

    //        ClassUnderTest.Attach(_request);
    //        template.Descriptor.As<ViewDescriptor>().Master.ShouldEqual(_templateRegistry.ElementAt(9));
    //    }

    //    [Test]
    //    public void fallback_to_master_in_host_2()
    //    {
    //        var template = _templateRegistry.ElementAt(10);
    //        _request.Template = template;

    //        ClassUnderTest.Attach(_request);
    //        template.Descriptor.As<ViewDescriptor>().Master.ShouldEqual(_templateRegistry.Last());
    //    }

    //    [Test]
    //    public void if_explicit_empty_master_then_binder_is_not_applied()
    //    {
    //        var template = _templateRegistry.ElementAt(3);
    //        _parsing.Master = string.Empty;
    //        _request.Template = template;

    //        ClassUnderTest.CanAttach(_request).ShouldBeFalse();
    //    }

    //    [Test]
    //    public void if_descriptor_is_not_viewdescriptor_then_binder_is_not_applied()
    //    {
    //        var template = _templateRegistry.ElementAt(12);
    //        _request.Template = template;
    //        ClassUnderTest.CanAttach(_request).ShouldBeFalse();
    //    }

    //    [Test]
    //    public void if_view_model_type_is_empty_and_master_is_not_set_then_binder_is_not_applied()
    //    {
    //        var template = _templateRegistry.ElementAt(11);
    //        _parsing.ViewModelType = string.Empty;
    //        _request.Template = template;
    //        _parsing.Master = "";
    //        ClassUnderTest.CanAttach(_request).ShouldBeFalse();
    //        _parsing.Master = null;
    //        ClassUnderTest.CanAttach(_request).ShouldBeFalse();
    //    }

    //    [Test]
    //    public void if_template_is_valid_for_binding_then_binder_can_be_applied()
    //    {
    //        var template = _templateRegistry.ElementAt(11);
    //        _request.Template = template;
    //        ClassUnderTest.CanAttach(_request).ShouldBeTrue();
    //    }

    //    [Test]
    //    public void if_master_is_already_set_binder_is_not_applied()
    //    {
    //        _request.Template = _templateRegistry.ElementAt(11);
    //        _request.Template.Descriptor.As<ViewDescriptor>().Master = _templateRegistry.ElementAt(14);
    //        ClassUnderTest.CanAttach(_request).ShouldBeFalse();
    //    }

    //    [Test]
    //    public void does_not_bind_partials()
    //    {
    //        _request.Template = _templateRegistry.ElementAt(13);
    //        ClassUnderTest.CanAttach(_request).ShouldBeFalse();
    //    }
    //}
}