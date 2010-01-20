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
                .For<StubController>().Layout("layout").Include("Index").Include("List.spark")
                .For<StubController>().Layout("elementLayout").Include("_Row");

            var assembly = _factory.Precompile(batch);

            Assert.IsNotNull(assembly);
            Assert.AreEqual(3, assembly.GetTypes().Length);
        }


        [Test]
        public void batch_descriptor_with_default_matching_rules()
        {
            var batch = new SparkBatchDescriptor();

            batch.For<StubController>();

            var descriptors = _factory.CreateDescriptors(batch);

            descriptors.ShouldHaveCount(2);
            descriptors[0].Templates.ShouldHaveCount(1);
            descriptors[1].Templates.ShouldHaveCount(1);
            descriptors.Any(descriptor => descriptor.Templates.Contains("Stub\\Index.spark")).ShouldBeTrue();
            descriptors.Any(descriptor => descriptor.Templates.Contains("Stub\\List.spark")).ShouldBeTrue();
        }

        [Test]
        public void batch_descriptor_with_exclude_rules()
        {
            var batch = new SparkBatchDescriptor();

            batch.For<StubController>().Include("*").Include("_*").Exclude("In*");

            var descriptors = _factory.CreateDescriptors(batch);

            descriptors.ShouldHaveCount(2);
            descriptors[0].Templates.ShouldHaveCount(1);
            descriptors[1].Templates.ShouldHaveCount(1);
            descriptors.Any(descriptor => descriptor.Templates.Contains("Stub\\_Row.spark")).ShouldBeTrue();
            descriptors.Any(descriptor => descriptor.Templates.Contains("Stub\\List.spark")).ShouldBeTrue();
        }

        [Test]
        public void batch_descriptor_with_multiple_layout_files()
        {
            var batch = new SparkBatchDescriptor();

            batch.For<StubController>().Layout("layout").Layout("anotherLayout").Include("Index").Include("List.spark");
            
            var assembly = _factory.Precompile(batch);
            assembly.ShouldNotBeNull();
            assembly.GetTypes().ShouldHaveCount(4);
        }

        [Test]
        public void batch_descriptor_with_wildcard_include_rules()
        {
            var batch = new SparkBatchDescriptor();

            batch
                .For<StubController>().Layout("layout").Include("*")
                .For<StubController>().Layout("elementLayout").Include("_*");

            var descriptors = _factory.CreateDescriptors(batch);
            descriptors.ShouldHaveCount(3);
            descriptors.Any(d => d.Templates.Contains("Stub\\Index.spark") && d.Templates.Contains("Shared\\layout.spark")).ShouldBeTrue();
            descriptors.Any(d => d.Templates.Contains("Stub\\List.spark") && d.Templates.Contains("Shared\\layout.spark")).ShouldBeTrue();
            descriptors.Any(d => d.Templates.Contains("Stub\\_Row.spark") && d.Templates.Contains("Shared\\elementLayout.spark")).ShouldBeTrue();

            var assembly = _factory.Precompile(batch);
            assembly.ShouldNotBeNull();
            assembly.GetTypes().ShouldHaveCount(3);
        }

        [Test]
        public void files_without_spark_extension_should_be_ignored()
        {
            _factory.ViewFolder = new InMemoryViewFolder
                                      {
                                          {"Stub\\Index.spark", "<p>index</p>"},
                                          {"Stub\\Foo.cs", "// some c# code"},
                                          {"Layouts\\Stub.spark", "<p>layout</p><use:view/>"},
                                      };
            var batch = new SparkBatchDescriptor();
            batch.For<StubController>();
            var descriptors = _factory.CreateDescriptors(batch);

            descriptors.ShouldHaveCount(1);
            descriptors[0].Templates.ShouldHaveCount(2);
            descriptors[0].Templates[0].ShouldEqual("Stub\\Index.spark");
            descriptors[0].Templates[1].ShouldEqual("Layouts\\Stub.spark");
        }
    }
}