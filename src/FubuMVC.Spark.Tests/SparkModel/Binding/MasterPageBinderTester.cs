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
        private BindContext _context;
        private IEnumerable<SparkItem> _sparkItems;

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
            Services.Inject<ISharedItemLocator>(new SharedItemLocator());
            _context = new BindContext 
			{ 
				AvailableItems = _sparkItems = createItems(), 
				Master = "application", 
				Tracer = MockFor<ISparkPackageTracer>() 
			};
        }

        private IEnumerable<SparkItem> createItems()
        {
            return new List<SparkItem>
            {
                newSpark(_pak1Root, Pak1, "Actions", "Controllers", "Home", "Home.spark"),
                newSpark(_pak1Root, Pak1, "Actions", "Handlers", "Products", "list.spark"),
                newSpark(_pak1Root, Pak1, "Actions", "Shared", "application.spark"),
                newSpark(_pak2Root, Pak2, "Features", "Controllers", "Home", "Home.spark"),
                newSpark(_pak2Root, Pak2, "Features", "Handlers", "Products", "list.spark"),
                newSpark(_pak2Root, Pak2, "Shared", "application.spark"),
                
                newSpark(_pak3Root, Pak3, "Features", "Controllers", "Home", "Home.spark"),
				
                newSpark(_hostRoot, Host, "Actions", "Shared", "application.spark"),
                newSpark(_hostRoot, Host, "Features", "Mixer", "chuck.spark"),
                newSpark(_hostRoot, Host, "Features", "Mixer", "Shared", "application.spark"),                
                newSpark(_hostRoot, Host, "Features", "roundkick.spark"),
                newSpark(_hostRoot, Host, "Handlers", "Products", "details.spark"),
				newSpark(_hostRoot, Host, "Shared", "bindings.xml"),				
                newSpark(_hostRoot, Host, "Shared", "_Partial.spark"),
				newSpark(_hostRoot, Host, "Shared", "application.spark")
            };
        }

        private SparkItem newSpark(string root, string origin, params string[] relativePaths)
        {
            var paths = new[]{root}.Union(relativePaths).ToArray();
            return new SparkItem(FileSystem.Combine(paths), root, origin);
        }

        [Test]
        public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_1()
        {
            var sparkItem = _sparkItems.First();
            ClassUnderTest.Bind(sparkItem, _context);
            _sparkItems.ElementAt(2).ShouldEqual(sparkItem.Master);
        }

        [Test]
        public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_2()
        {
            var sparkItem = _sparkItems.ElementAt(3);
            ClassUnderTest.Bind(sparkItem, _context);
            _sparkItems.ElementAt(5).ShouldEqual(sparkItem.Master);
        }

        [Test]
        public void fallback_to_master_in_shared_host_when_no_local_ancestor_exists()
        {
            var sparkItem = _sparkItems.ElementAt(6);
            ClassUnderTest.Bind(sparkItem, _context);
            _sparkItems.Last().ShouldEqual(sparkItem.Master);
        }

        [Test]
        public void fallback_to_master_in_host_1()
        {
            var sparkItem = _sparkItems.ElementAt(8);
            ClassUnderTest.Bind(sparkItem, _context);
            _sparkItems.ElementAt(9).ShouldEqual(sparkItem.Master);
        }

        [Test]
        public void fallback_to_master_in_host_2()
        {
            var sparkItem = _sparkItems.ElementAt(10);
            ClassUnderTest.Bind(sparkItem, _context);
            _sparkItems.Last().ShouldEqual(sparkItem.Master);
        }

        [Test]
        public void if_item_is_a_master_page_it_is_not_bound_to_itself()
        {
			var item = _sparkItems.Last();
            ClassUnderTest.Bind(item, _context);
			item.Master.ShouldBeNull();
        }

        [Test]
		public void	if_explicit_empty_master_then_binder_is_not_applied()
		{
			ClassUnderTest.CanBind(_sparkItems.ElementAt(3), new BindContext{Master = string.Empty}).ShouldBeFalse();	
		}
		
		[Test]
		public void	if_spark_item_is_not_normal_view_then_binder_is_not_applied()
		{
			ClassUnderTest.CanBind(_sparkItems.ElementAt(12), _context).ShouldBeFalse();	
		}
		
		[Test]
		public void	if_spark_item_is_partial_then_binder_is_not_applied()
		{
			ClassUnderTest.CanBind(_sparkItems.ElementAt(13), new BindContext{Master = null}).ShouldBeFalse();	
		}
    }
}