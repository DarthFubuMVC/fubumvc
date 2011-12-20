using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class MasterAttacherTester : InteractionContext<MasterAttacher>
    {
        private AttachRequest _request;
        private ViewDescriptor _viewDescriptor;
        private Parsing _parsing;
        private ITemplate _template;

        protected override void beforeEach()
        {
            _template = new Template("b/a.spark", "b", "c")
            {
                Descriptor = _viewDescriptor = new ViewDescriptor(_template)
                {
                    ViewModel = typeof(ProductModel)
                }
            };
            
            _parsing = new Parsing
            {
                Master = "application",
                ViewModelType = _viewDescriptor.ViewModel.FullName
            };

            _request = new AttachRequest
            {
                Template = _template,
                Logger = MockFor<ISparkLogger>()
            };

            MockFor<IParsingRegistrations>().Expect(x => x.ParsingFor(_template)).Return(_parsing);
        }

        [Test]
        public void if_template_is_valid_for_attachment_then_attacher_is_applied()
        {
            ClassUnderTest.CanAttach(_request).ShouldBeTrue();
        }

        [Test]
        public void if_explicit_empty_master_then_binder_is_not_applied()
        {
            _parsing.Master = string.Empty;
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_descriptor_is_not_viewdescriptor_then_binder_is_not_applied()
        {
            _template.Descriptor = new NulloDescriptor();
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_view_model_type_is_null_and_master_is_invalid_then_binder_is_not_applied_1()
        {
            _viewDescriptor.ViewModel = null;            
            _parsing.Master = null;

            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_view_model_type_is_null_and_master_is_invalid_then_binder_is_not_applied_2()
        {
            _viewDescriptor.ViewModel = null;
            _parsing.Master = "";

            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_master_is_already_set_binder_is_not_applied()
        {
            _viewDescriptor.Master = MockFor<ITemplate>();
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_partials()
        {
            _request.Template = new Template("b/_partial.spark", "b", "c");
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void when_master_is_not_set_fallback_is_used_by_locator()
        {
            _parsing.Master = null;
            ClassUnderTest.Attach(_request);

            MockFor<ISharedTemplateLocator>()
                .AssertWasCalled(x => x.LocateMaster(ClassUnderTest.MasterName, _template));
        }

        [Test]
        public void when_master_is_set_it_is_used_by_locator()
        {
            ClassUnderTest.Attach(_request);
            MockFor<ISharedTemplateLocator>()
                .AssertWasCalled(x => x.LocateMaster(_parsing.Master, _template));
        }

        [Test]
        public void when_no_master_is_found_it_is_logged()
        {
            ClassUnderTest.Attach(_request);
            verify_log_contains("not found");
        }

        [Test]
        public void when_master_is_found_it_is_set_on_view_descriptor()
        {
            master_is_found();
            ClassUnderTest.Attach(_request);
            _viewDescriptor.Master.ShouldEqual(MockFor<ITemplate>());
        }

        [Test]
        public void when_master_is_found_it_is_logged()
        {
            master_is_found();
            ClassUnderTest.Attach(_request);
            verify_log_contains("found at");
        }


        private void verify_log_contains(string snippet)
        {
            MockFor<ISparkLogger>()
                .AssertWasCalled(x => x.Log(Arg<ITemplate>.Is.Equal(_template), Arg<string>.Matches(s => s.Contains(snippet))));            
        }

        private void master_is_found()
        {
            MockFor<ISharedTemplateLocator>()
                .Stub(x => x.LocateMaster(_parsing.Master, _template))
                .Return(MockFor<ITemplate>());            
        }
    }

    // UPRADE: Move to some other lower place of what can be used from this, make sure coverage is same.

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


    //}
}