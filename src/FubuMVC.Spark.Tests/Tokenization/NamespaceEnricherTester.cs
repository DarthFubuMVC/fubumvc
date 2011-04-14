using System.IO;
using FubuMVC.Spark.Tokenization;
using FubuMVC.Spark.Tokenization.Model;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Tokenization
{
    public class NamespaceEnricherTester
    {
        private string _root;
        private NamespaceEnricher _enricher;

        [TestFixtureSetUp]
        public void Setup()
        {
            var vol = "c" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
            _root = Path.Combine(vol, "inetput", "www", "web");
            _enricher = new NamespaceEnricher();
        }

        [Test]
        public void namespace_is_set_correctly()
        {
            var path = Path.Combine(_root, "controllers", "home", "home.spark");
            var file = new SparkFile(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            _enricher.Alter(file);
            Assert.AreEqual("FubuMVC.Spark.Tests.controllers.home", file.Namespace);
        }

        [Test]
        public void namespace_of_files_in_root_is_set_correctly()
        {
            var path = Path.Combine(_root, "home.spark");
            var file = new SparkFile(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            _enricher.Alter(file);
            Assert.AreEqual("FubuMVC.Spark.Tests", file.Namespace);
        }

    }

    public class FooViewModel
    {
    } 

}