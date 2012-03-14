//using System.IO;
//using FubuCore;
//using FubuMVC.Razor.RazorModel;
//using FubuMVC.Razor.RazorModel;
//using FubuTestingSupport;
//using NUnit.Framework;

//namespace FubuMVC.Razor.Tests.RazorModel.Policies
//{
//    [TestFixture]
//    public class NamespacePolicyTester : InteractionContext<NamespacePolicy>
//    {
//        private string _root;

//        [TestFixtureSetUp]
//        public void Setup()
//        {
//            var vol = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
//            _root = Path.Combine(vol, "inetput", "www", "web");
//        }

//        [Test]
//        public void namespace_is_set_correctly()
//        {
//            var path = Path.Combine(_root, "controllers", "home", "home.cshtml");
//            var item = new Template(path, _root, "");
//            item.Descriptor = new ViewDescriptor<IRazorTemplate>(item)
//            {
//                ViewModel = typeof (FooViewModel)
//            };
            
//            ClassUnderTest.Apply(item);
//            item.Descriptor.As<ViewDescriptor<IRazorTemplate>>().Namespace.ShouldEqual("FubuMVC.Razor.Tests.controllers.home");
//        }

//        [Test]
//        public void namespace_of_files_in_root_is_set_correctly()
//        {
//            var path = Path.Combine(_root, "home.cshtml");
//            var item = new Template(path, _root, "");
//            item.Descriptor = new ViewDescriptor<IRazorTemplate>(item)
//            {
//                ViewModel = typeof(FooViewModel)
//            };
            
//            ClassUnderTest.Apply(item);
//            item.Descriptor.As<ViewDescriptor<IRazorTemplate>>().Namespace.ShouldEqual("FubuMVC.Razor.Tests");
//        }

//        [Test]
//        public void it_matches_if_item_has_viewmodel_and_namespace_is_empty_positive()
//        {
//            var path = Path.Combine(_root, "home.cshtml");
//            var item = new Template(path, _root, "");
//            item.Descriptor = new ViewDescriptor<IRazorTemplate>(item)
//            {
//                ViewModel = typeof(FooViewModel)
//            };
			
//            ClassUnderTest.Matches(item).ShouldBeTrue();
//        }		
		
//        [Test]
//        public void it_matches_if_item_has_viewmodel_and_namespace_is_empty_negative_1()
//        {
//            var path = Path.Combine(_root, "home.cshtml");
//            var item = new Template(path, _root, "");
			
//            ClassUnderTest.Matches(item).ShouldBeFalse();
//        }		
		
//        [Test]
//        public void it_matches_if_item_has_viewmodel_and_namespace_is_empty_negative_2()
//        {
//            var path = Path.Combine(_root, "home.cshtml");
//            var item = new Template(path, _root, "");
//            item.Descriptor = new ViewDescriptor<IRazorTemplate>(item)
//            {
//                ViewModel = typeof(FooViewModel),
//                Namespace = "Someone.Else.Did.This" 
//            };

//            ClassUnderTest.Matches(item).ShouldBeFalse();
//        }	

//        [Test]
//        public void it_does_not_match_nullodescriptor()
//        {
//            var path = Path.Combine(_root, "home.cshtml");
//            var item = new Template(path, _root, "")
//            {
//                Descriptor = new NulloDescriptor()
//            };

//            ClassUnderTest.Matches(item).ShouldBeFalse();            
//        }
//    }

//    public class FooViewModel {}
//}