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
    }
}