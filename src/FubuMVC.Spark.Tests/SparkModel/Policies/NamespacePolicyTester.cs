using System.IO;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.SparkModel.Policies
{
    [TestFixture]
    public class NamespacePolicyTester : InteractionContext<NamespacePolicy>
    {
        private const string Root = "web";
        private StubTemplate _template;
        protected override void beforeEach()
        {
            _template = new StubTemplate{
                Descriptor = new SparkDescriptor(_template, new SparkViewEngine())
            {
                ViewModel = typeof(FooViewModel)
            },
                RootPath = Root,
                FilePath = Path.Combine(Root, "home.spark")
            };
        }


        [Test]
        public void namespace_of_files_that_are_located_in_nested_directory_is_set_correctly()
        {
            _template.FilePath = Path.Combine(Root, "a", "b", "c", "home.spark");
            ClassUnderTest.Apply(_template);
            _template.Descriptor.As<SparkDescriptor>().Namespace.ShouldEqual("FubuMVC.Spark.Tests.a.b.c");
        }

        [Test]
        public void namespace_of_files_in_root_is_set_correctly()
        {
            ClassUnderTest.Apply(_template);
            _template.Descriptor.As<SparkDescriptor>().Namespace.ShouldEqual("FubuMVC.Spark.Tests");
        }

        [Test]
        public void it_matches_if_item_has_viewmodel_and_namespace_is_empty_positive()
        {
			ClassUnderTest.Matches(_template).ShouldBeTrue();
        }		
		
		[Test]
        public void it_matches_if_item_has_viewmodel_and_namespace_is_empty_negative_1()
		{
            _template.Descriptor = null;
			ClassUnderTest.Matches(_template).ShouldBeFalse();
        }		
		
		[Test]
        public void it_matches_if_item_has_viewmodel_and_namespace_is_empty_negative_2()
		{
            _template.Descriptor.As<SparkDescriptor>().Namespace = "Someone.Else.Did.This";
            ClassUnderTest.Matches(_template).ShouldBeFalse();
        }	

        [Test]
        public void it_does_not_match_nullodescriptor()
        {
            _template.Descriptor = new NulloDescriptor();
            ClassUnderTest.Matches(_template).ShouldBeFalse();            
        }
    }

    public class FooViewModel {}
}