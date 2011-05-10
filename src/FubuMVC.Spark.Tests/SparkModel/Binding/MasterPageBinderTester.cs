using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
    [TestFixture]
    public class MasterPageBinderTester : InteractionContext<MasterPageBinder>
    {
        private BindRequest _request;
        private IEnumerable<Template> _templates;

        const string Host = FubuSparkConstants.HostOrigin;
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
            _request = new BindRequest 
			{ 
				Templates = _templates = createTemplates(), 
				Master = "application", 
				Logger = MockFor<ISparkLogger>() 
			};
        }

        private IEnumerable<Template> createTemplates()
        {
            return new List<Template>
            {
                newTemplate(_pak1Root, Pak1, "Actions", "Controllers", "Home", "Home.spark"),
                newTemplate(_pak1Root, Pak1, "Actions", "Handlers", "Products", "list.spark"),
                newTemplate(_pak1Root, Pak1, "Actions", "Shared", "application.spark"),
                newTemplate(_pak2Root, Pak2, "Features", "Controllers", "Home", "Home.spark"),
                newTemplate(_pak2Root, Pak2, "Features", "Handlers", "Products", "list.spark"),
                newTemplate(_pak2Root, Pak2, "Shared", "application.spark"),
                
                newTemplate(_pak3Root, Pak3, "Features", "Controllers", "Home", "Home.spark"),
				
                newTemplate(_hostRoot, Host, "Actions", "Shared", "application.spark"),
                newTemplate(_hostRoot, Host, "Features", "Mixer", "chuck.spark"),
                newTemplate(_hostRoot, Host, "Features", "Mixer", "Shared", "application.spark"),                
                newTemplate(_hostRoot, Host, "Features", "roundkick.spark"),
                newTemplate(_hostRoot, Host, "Handlers", "Products", "details.spark"),
				newTemplate(_hostRoot, Host, "Shared", "bindings.xml"),				
                newTemplate(_hostRoot, Host, "Shared", "_Partial.spark"),
				newTemplate(_hostRoot, Host, "Shared", "application.spark")
            };
        }

        private Template newTemplate(string root, string origin, params string[] relativePaths)
        {
            var paths = new[]{root}.Union(relativePaths).ToArray();
            var template = new Template(FileSystem.Combine(paths), root, origin);
            template.Descriptor = new ViewDescriptor(template);            
            return template;
        }

        [Test]
        public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_1()
        {
            var template = _templates.First();
            _request.Target = template;
            
            ClassUnderTest.Bind(_request);
            _templates.ElementAt(2).ShouldEqual(template.Descriptor.As<ViewDescriptor>().Master);
        }

        [Test]
        public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_2()
        {
            var template = _templates.ElementAt(3);
            _request.Target = template;
            
            ClassUnderTest.Bind(_request);
            _templates.ElementAt(5).ShouldEqual(template.Descriptor.As<ViewDescriptor>().Master);
        }

        [Test]
        public void fallback_to_master_in_shared_host_when_no_local_ancestor_exists()
        {
            var template = _templates.ElementAt(6);
            _request.Target = template;
            
            ClassUnderTest.Bind(_request);
            _templates.Last().ShouldEqual(template.Descriptor.As<ViewDescriptor>().Master);
        }

        [Test]
        public void fallback_to_master_in_host_1()
        {
            var template = _templates.ElementAt(8);
            _request.Target = template;

            ClassUnderTest.Bind(_request);
            _templates.ElementAt(9).ShouldEqual(template.Descriptor.As<ViewDescriptor>().Master);
        }

        [Test]
        public void fallback_to_master_in_host_2()
        {
            var template = _templates.ElementAt(10);
            _request.Target = template;

            ClassUnderTest.Bind(_request);
            _templates.Last().ShouldEqual(template.Descriptor.As<ViewDescriptor>().Master);
        }

        [Test]
        public void if_item_is_a_master_page_it_is_not_bound_to_itself()
        {
			var template = _templates.Last();
            _request.Target = template;

            ClassUnderTest.Bind(_request);
            template.Descriptor.As<ViewDescriptor>().Master.ShouldBeNull();
        }

        [Test]
		public void	if_explicit_empty_master_then_binder_is_not_applied()
        {
            var template = _templates.ElementAt(3);
            _request.Master = string.Empty;
            _request.Target = template;

			ClassUnderTest.CanBind(_request).ShouldBeFalse();	
		}
		
		[Test]
		public void	if_spark_item_is_not_normal_view_then_binder_is_not_applied()
		{
            var template = _templates.ElementAt(12);
            _request.Target = template;

			ClassUnderTest.CanBind(_request).ShouldBeFalse();	
		}
		
		[Test]
		public void	if_spark_item_is_partial_then_binder_is_not_applied()
		{
            var template = _templates.ElementAt(13);
		    _request.Master = null;
            _request.Target = template;

			ClassUnderTest.CanBind(_request).ShouldBeFalse();	
		}
    }
}