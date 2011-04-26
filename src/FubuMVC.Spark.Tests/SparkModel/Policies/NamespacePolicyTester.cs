using System.IO;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binders
{
    [TestFixture]
    public class NamespacePolicyTester : InteractionContext<NamespacePolicy>
    {
        private string _root;

        [TestFixtureSetUp]
        public void Setup()
        {
            var vol = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            _root = Path.Combine(vol, "inetput", "www", "web");
        }

        [Test]
        public void namespace_is_set_correctly()
        {
            var path = Path.Combine(_root, "controllers", "home", "home.spark");
            var item = new SparkItem(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            
            ClassUnderTest.Apply(item);
            item.Namespace.ShouldEqual("FubuMVC.Spark.Tests.controllers.home");
        }

        [Test]
        public void namespace_of_files_in_root_is_set_correctly()
        {
            var path = Path.Combine(_root, "home.spark");
            var item = new SparkItem(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            
            ClassUnderTest.Apply(item);
            item.Namespace.ShouldEqual("FubuMVC.Spark.Tests");
        }

        // TODO : Edge cases, boundaries

    }

    public class FooViewModel
    {
    } 

}