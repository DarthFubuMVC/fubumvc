using System.Linq;
using NUnit.Framework;
using Spark.FileSystem;
using Spark.Web.FubuMVC.Tests.Controllers;

namespace Spark.Web.FubuMVC.Tests
{
    [TestFixture]
    public class SparkBatchCompilationTester
    {
        private SparkViewFactory _factory;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var settings = new SparkSettings();

            _factory = new SparkViewFactory(settings) { ViewFolder = new FileSystemViewFolder("FubuMVC.Tests.Views") };
        }

        #endregion

        [Test]
        public void can_compile_spark_batch_descriptor()
        {
            var batch = new SparkBatchDescriptor();
            batch
                .For<BatchController>().Layout("Layout").Include("Index").Include("List.spark")
                .For<BatchController>().Layout("ElementLayout").Include("_row");

            var assembly = _factory.Precompile(batch);

            Assert.IsNotNull(assembly);
            Assert.AreEqual(3, assembly.GetTypes().Length);
        }


        [Test]
        public void batch_descriptor_with_default_matching_rules()
        {
            var batch = new SparkBatchDescriptor();

            batch.For<BatchController>();

            var descriptors = _factory.CreateDescriptors(batch);

            descriptors.ShouldHaveCount(2);
            descriptors[0].Templates.ShouldHaveCount(1);
            descriptors[1].Templates.ShouldHaveCount(1);
            descriptors.Any(descriptor => descriptor.Templates.Contains("Batch\\Index.spark")).ShouldBeTrue();
            descriptors.Any(descriptor => descriptor.Templates.Contains("Batch\\List.spark")).ShouldBeTrue();
        }

        [Test]
        public void batch_descriptor_with_exclude_rules()
        {
            var batch = new SparkBatchDescriptor();

            batch.For<BatchController>().Include("*").Include("_*").Exclude("In*");

            var descriptors = _factory.CreateDescriptors(batch);

            descriptors.ShouldHaveCount(2);
            descriptors[0].Templates.ShouldHaveCount(1);
            descriptors[1].Templates.ShouldHaveCount(1);
            descriptors.Any(descriptor => descriptor.Templates.Contains("Batch\\_row.spark")).ShouldBeTrue();
            descriptors.Any(descriptor => descriptor.Templates.Contains("Batch\\List.spark")).ShouldBeTrue();
        }

        [Test]
        public void batch_descriptor_with_multiple_layout_files()
        {
            var batch = new SparkBatchDescriptor();

            batch.For<BatchController>().Layout("Layout").Layout("AnotherLayout").Include("Index").Include("List.spark");
            
            var assembly = _factory.Precompile(batch);
            assembly.ShouldNotBeNull();
            assembly.GetTypes().ShouldHaveCount(4);
        }

        [Test]
        public void batch_descriptor_with_wildcard_include_rules()
        {
            var batch = new SparkBatchDescriptor();

            batch
                .For<BatchController>().Layout("Layout").Include("*")
                .For<BatchController>().Layout("ElementLayout").Include("_*");

            var descriptors = _factory.CreateDescriptors(batch);
            descriptors.ShouldHaveCount(3);
            descriptors.Any(d => d.Templates.Contains("Batch\\Index.spark") && d.Templates.Contains("Shared\\Layout.spark")).ShouldBeTrue();
            descriptors.Any(d => d.Templates.Contains("Batch\\List.spark") && d.Templates.Contains("Shared\\Layout.spark")).ShouldBeTrue();
            descriptors.Any(d => d.Templates.Contains("Batch\\_row.spark") && d.Templates.Contains("Shared\\ElementLayout.spark")).ShouldBeTrue();

            var assembly = _factory.Precompile(batch);
            assembly.ShouldNotBeNull();
            assembly.GetTypes().ShouldHaveCount(3);
        }

        [Test]
        public void files_without_spark_extension_should_be_ignored()
        {
            _factory.ViewFolder = new InMemoryViewFolder
                                      {
                                          {"Batch\\Index.spark", "<p>index</p>"},
                                          {"Batch\\Foo.cs", "// some c# code"},
                                          {"Layouts\\Batch.spark", "<p>layout</p><use:view/>"},
                                      };
            var batch = new SparkBatchDescriptor();
            batch.For<BatchController>();
            var descriptors = _factory.CreateDescriptors(batch);

            descriptors.ShouldHaveCount(1);
            descriptors[0].Templates.ShouldHaveCount(2);
            descriptors[0].Templates[0].ShouldEqual("Batch\\Index.spark");
            descriptors[0].Templates[1].ShouldEqual("Layouts\\Batch.spark");
        }
    }
}