using System.IO;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Policies
{
    [TestFixture]
    public class NamespacePolicyTester : InteractionContext<NamespacePolicy>
    {
        private const string Root = "web";
        private string _path;
        private ITemplate _template;
        private ISparkDescriptor _viewDescriptor;
        protected override void beforeEach()
        {
            _path = Path.Combine(Root, "home.spark");
            _template = MockFor<ITemplate>();
            _viewDescriptor = new ViewDescriptor(_template)
            {
                ViewModel = typeof(FooViewModel)
            };
            _template.Stub(x => x.Descriptor).Return(null).WhenCalled(x => x.ReturnValue = _viewDescriptor);
            _template.Stub(x => x.RootPath).Return(Root);
            _template.Stub(x => x.FilePath).Return(null).WhenCalled(x => x.ReturnValue = _path);
        }


        [Test]
        public void namespace_of_files_that_are_located_in_nested_directory_is_set_correctly()
        {
            _path = Path.Combine(Root, "a", "b", "c", "home.spark");
            ClassUnderTest.Apply(_template);
            _viewDescriptor.As<ViewDescriptor>().Namespace.ShouldEqual("FubuMVC.Spark.Tests.a.b.c");
        }

        [Test]
        public void namespace_of_files_in_root_is_set_correctly()
        {
            ClassUnderTest.Apply(_template);
            _viewDescriptor.As<ViewDescriptor>().Namespace.ShouldEqual("FubuMVC.Spark.Tests");
        }

        [Test]
        public void it_matches_if_item_has_viewmodel_and_namespace_is_empty_positive()
        {
			ClassUnderTest.Matches(_template).ShouldBeTrue();
        }		
		
		[Test]
        public void it_matches_if_item_has_viewmodel_and_namespace_is_empty_negative_1()
		{
		    _viewDescriptor = null;
			ClassUnderTest.Matches(_template).ShouldBeFalse();
        }		
		
		[Test]
        public void it_matches_if_item_has_viewmodel_and_namespace_is_empty_negative_2()
		{
		    _viewDescriptor.As<ViewDescriptor>().Namespace = "Someone.Else.Did.This";
            ClassUnderTest.Matches(_template).ShouldBeFalse();
        }	

        [Test]
        public void it_does_not_match_nullodescriptor()
        {
            _viewDescriptor = new NulloDescriptor();
            ClassUnderTest.Matches(_template).ShouldBeFalse();            
        }
    }

    public class FooViewModel {}
}