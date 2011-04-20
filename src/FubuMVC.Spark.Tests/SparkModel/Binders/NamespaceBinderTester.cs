using System.IO;
using FubuMVC.Spark.SparkModel;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binders
{
    [TestFixture]
    public class NamespaceBinderTester
    {
        private string _root;
        private NamespaceBinder _binder;
        private BindContext _context;

        [TestFixtureSetUp]
        public void Setup()
        {
            var vol = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            _root = Path.Combine(vol, "inetput", "www", "web");

            _binder = new NamespaceBinder();
            _context = new BindContext();
        }

        [Test]
        public void namespace_is_set_correctly()
        {
            var path = Path.Combine(_root, "controllers", "home", "home.spark");
            var item = new SparkItem(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            
            _binder.Bind(item, _context);
            Assert.AreEqual("FubuMVC.Spark.Tests.controllers.home", item.Namespace);
        }

        [Test]
        public void namespace_of_files_in_root_is_set_correctly()
        {
            var path = Path.Combine(_root, "home.spark");
            var item = new SparkItem(path, _root, "") { ViewModelType = typeof(FooViewModel) };
            
            _binder.Bind(item, _context);
            Assert.AreEqual("FubuMVC.Spark.Tests", item.Namespace);
        }

        // TODO : Edge cases, boundaries

    }

    public class FooViewModel
    {
    } 

}