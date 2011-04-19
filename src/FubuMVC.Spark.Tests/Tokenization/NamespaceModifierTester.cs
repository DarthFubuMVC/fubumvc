using System.IO;
using FubuMVC.Spark.Tokenization;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Tokenization
{
    [TestFixture]
    public class NamespaceModifierTester
    {
        private string _root;
        private NamespaceModifier _modifier;
        private ModificationContext _context;

        [TestFixtureSetUp]
        public void Setup()
        {
            var vol = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            _root = Path.Combine(vol, "inetput", "www", "web");

            _modifier = new NamespaceModifier();
            _context = new ModificationContext();
        }

        [Test]
        public void namespace_is_set_correctly()
        {
            var path = Path.Combine(_root, "controllers", "home", "home.spark");
            var item = new SparkItem(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            
            _modifier.Modify(item, _context);
            Assert.AreEqual("FubuMVC.Spark.Tests.controllers.home", item.Namespace);
        }

        [Test]
        public void namespace_of_files_in_root_is_set_correctly()
        {
            var path = Path.Combine(_root, "home.spark");
            var item = new SparkItem(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            
            _modifier.Modify(item, _context);
            Assert.AreEqual("FubuMVC.Spark.Tests", item.Namespace);
        }

        // TODO : Edge cases, boundaries

    }

    public class FooViewModel
    {
    } 

}