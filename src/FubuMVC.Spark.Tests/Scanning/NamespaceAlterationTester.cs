using System.IO;
using FubuMVC.Spark.Tokenization;
using FubuMVC.Spark.Tokenization.Model;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Scanning
{
    public class NamespaceAlterationTester
    {
        private string _root;
        private NamespaceAlteration _alteration;

        [TestFixtureSetUp]
        public void Setup()
        {
            var vol = "c" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
            _root = Path.Combine(vol, "inetput", "www", "web");
            _alteration = new NamespaceAlteration(new NamespaceResolver());
        }

        [Test]
        public void namespace_is_set_correctly()
        {
            var path = Path.Combine(_root, "controllers", "home", "home.spark");
            var file = new SparkFile(path, _root, "") { ViewModel = typeof(FooViewModel) };
            _alteration.Alter(file);
            Assert.AreEqual("FubuMVC.Spark.Tests.controllers.home", file.Namespace);
        }

        [Test]
        public void namespace_of_files_in_root_is_set_correctly()
        {
            var path = Path.Combine(_root, "home.spark");
            var file = new SparkFile(path, _root, "") { ViewModel = typeof(FooViewModel) };
            _alteration.Alter(file);
            Assert.AreEqual("FubuMVC.Spark.Tests", file.Namespace);
        }

    }

    public class FooViewModel
    {
    } 

}