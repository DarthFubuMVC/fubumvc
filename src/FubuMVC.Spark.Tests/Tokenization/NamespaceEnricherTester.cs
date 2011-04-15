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
        private EnrichmentContext _context;

        [TestFixtureSetUp]
        public void Setup()
        {
            var vol = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            _root = Path.Combine(vol, "inetput", "www", "web");

            _enricher = new NamespaceEnricher();
            _context = new EnrichmentContext();
        }

        [Test]
        public void namespace_is_set_correctly()
        {
            var path = Path.Combine(_root, "controllers", "home", "home.spark");
            var file = new SparkFile(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            
            _enricher.Enrich(file, _context);
            Assert.AreEqual("FubuMVC.Spark.Tests.controllers.home", file.Namespace);
        }

        [Test]
        public void namespace_of_files_in_root_is_set_correctly()
        {
            var path = Path.Combine(_root, "home.spark");
            var file = new SparkFile(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            
            _enricher.Enrich(file, _context);
            Assert.AreEqual("FubuMVC.Spark.Tests", file.Namespace);
        }

        // TODO : Edge cases, boundaries

    }

    public class FooViewModel
    {
    } 

}