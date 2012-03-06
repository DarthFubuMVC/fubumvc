using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests.RazorModel.Binding
{
    [TestFixture]
    public class MasterPageBinderTester : InteractionContext<MasterPageBinder>
    {
        private BindRequest<IRazorTemplate> _request;
        private TemplateRegistry<IRazorTemplate> _templateRegistry;

        const string Host = FubuRazorConstants.HostOrigin;
        const string Pak1 = "pak1";
        const string Pak2 = "pak2";
        const string Pak3 = "pak3";

        private readonly string _hostRoot;
        private readonly string _pak1Root;
        private readonly string _pak2Root;
        private readonly string _pak3Root;

        public MasterPageBinderTester()
        {
            _hostRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inetpub", "www", "web");
            _pak1Root = Path.Combine(_hostRoot, Pak1);
            _pak2Root = Path.Combine(_hostRoot, Pak2);
            _pak3Root = Path.Combine(_hostRoot, Pak3);
        }

        protected override void beforeEach()
        {           
            Services.Inject<ISharedTemplateLocator>(new SharedTemplateLocator());
            _request = new BindRequest<IRazorTemplate> 
			{ 
				TemplateRegistry = _templateRegistry = createTemplates(),
                Parsing = new Parsing
                {
				    Master = "application", 
                    ViewModelType = typeof(ProductModel).FullName
                },
				Logger = MockFor<ITemplateLogger>(),
			};
        }

        private TemplateRegistry<IRazorTemplate> createTemplates()
        {
            return new TemplateRegistry<IRazorTemplate>
            {
                newTemplate(_pak1Root, Pak1, true, "Actions", "Controllers", "Home", "Home.cshtml"), // 0
                newTemplate(_pak1Root, Pak1, true, "Actions", "Handlers", "Products", "list.cshtml"), // 1
                newTemplate(_pak1Root, Pak1, false, "Actions", "Shared", "application.cshtml"), // 2
                newTemplate(_pak2Root, Pak2, true, "Features", "Controllers", "Home", "Home.cshtml"), // 3
                newTemplate(_pak2Root, Pak2, true, "Features", "Handlers", "Products", "list.cshtml"), // 4
                newTemplate(_pak2Root, Pak2, false, "Shared", "application.cshtml"), // 5
                
                newTemplate(_pak3Root, Pak3, true, "Features", "Controllers", "Home", "Home.cshtml"), // 6
				
                newTemplate(_hostRoot, Host, false, "Actions", "Shared", "application.cshtml"), // 7
                newTemplate(_hostRoot, Host, true, "Features", "Mixer", "chuck.cshtml"), // 8
                newTemplate(_hostRoot, Host, false, "Features", "Mixer", "Shared", "application.cshtml"), // 9
                newTemplate(_hostRoot, Host, true, "Features", "roundkick.cshtml"), // 10
                newTemplate(_hostRoot, Host, true, "Handlers", "Products", "details.cshtml"), // 11
				newTemplate(_hostRoot, Host, false, "Shared", "bindings.xml"), // 12
                newTemplate(_hostRoot, Host, false, "Shared", "_Partial.cshtml"), // 13
				newTemplate(_hostRoot, Host, false, "Shared", "application.cshtml") // 14
            };
        }

        private static IRazorTemplate newTemplate(string root, string origin, bool isView, params string[] relativePaths)
        {
            var paths = new[] { root }.Union(relativePaths).ToArray();
            var template = new Template(FubuCore.FileSystem.Combine(paths), root, origin);
            if (isView)
            {
                template.Descriptor = new ViewDescriptor(template);
            }
            return template;
        }

        [Test]
        public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_1()
        {
            var template = _templateRegistry.First();
            _request.Target = template;
            
            ClassUnderTest.Bind(_request);
            _templateRegistry.ElementAt(2).ShouldEqual(template.Descriptor.As<ViewDescriptor>().Master);
        }

        [Test]
        public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_2()
        {
            var template = _templateRegistry.ElementAt(3);
            _request.Target = template;
            
            ClassUnderTest.Bind(_request);
            template.Descriptor.As<ViewDescriptor>().Master.ShouldEqual(_templateRegistry.ElementAt(5));
        }

        [Test]
        public void fallback_to_master_in_shared_host_when_no_local_ancestor_exists()
        {
            var template = _templateRegistry.ElementAt(6);
            _request.Target = template;
            
            ClassUnderTest.Bind(_request);
            template.Descriptor.As<ViewDescriptor>().Master.ShouldEqual(_templateRegistry.Last());
        }

        [Test]
        public void fallback_to_master_in_host_1()
        {
            var template = _templateRegistry.ElementAt(8);
            _request.Target = template;

            ClassUnderTest.Bind(_request);
            template.Descriptor.As<ViewDescriptor>().Master.ShouldEqual(_templateRegistry.ElementAt(9));
        }

        [Test]
        public void fallback_to_master_in_host_2()
        {
            var template = _templateRegistry.ElementAt(10);
            _request.Target = template;

            ClassUnderTest.Bind(_request);
            template.Descriptor.As<ViewDescriptor>().Master.ShouldEqual(_templateRegistry.Last());
        }

        [Test]
		public void	if_explicit_empty_master_then_binder_is_not_applied()
        {
            var template = _templateRegistry.ElementAt(3);
            _request.Parsing.Master = string.Empty;
            _request.Target = template;

			ClassUnderTest.CanBind(_request).ShouldBeFalse();	
		}

        [Test]
        public void if_descriptor_is_not_viewdescriptor_then_binder_is_not_applied()
        {
            var template = _templateRegistry.ElementAt(12);
            _request.Target = template;
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void if_view_model_type_is_empty_and_master_is_not_set_then_binder_is_not_applied()
        {
            var template = _templateRegistry.ElementAt(11);
            _request.Parsing.ViewModelType = string.Empty;
            _request.Target = template;
            _request.Parsing.Master = "";
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
            _request.Parsing.Master = null;
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void if_template_is_valid_for_binding_then_binder_can_be_applied()
        {
            var template = _templateRegistry.ElementAt(11);
            _request.Target = template;
            ClassUnderTest.CanBind(_request).ShouldBeTrue();
        }

        [Test]
        public void if_master_is_already_set_binder_is_not_applied()
        {
            _request.Target = _templateRegistry.ElementAt(11);
            _request.Target.Descriptor.As<ViewDescriptor>().Master = _templateRegistry.ElementAt(14);
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_partials()
        {
            _request.Target = _templateRegistry.ElementAt(13);
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

    }
}