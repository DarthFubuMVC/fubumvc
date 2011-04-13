using System.IO;
using FubuMVC.Spark.Scanning;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Scanning
{
    public class NamespaceAlterationTester
    {
        private string _root;

        [TestFixtureSetUp]
        public void Setup()
        {
            var vol = "c" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
            _root = Path.Combine(vol, "inetput", "www", "web");
        }

        [Test]
        public void namespace_is_set_correctly()
        {
            var alteration = new NamespaceAlteration();
            var path = Path.Combine(_root, "controllers", "home", "home.spark");
            var file = new SparkFile(path, _root, "") { ViewModel = typeof(FooViewModel) };
            alteration.Alter(file);
            Assert.AreEqual("FubuMVC.Spark.Tests.controllers.home", file.Namespace);
        }

        [Test]
        public void namespace_of_files_in_root_is_set_correctly()
        {
            var alteration = new NamespaceAlteration();
            var path = Path.Combine(_root, "home.spark");
            var file = new SparkFile(path, _root, "") { ViewModel = typeof(FooViewModel) };
            alteration.Alter(file);
            Assert.AreEqual("FubuMVC.Spark.Tests", file.Namespace);
        }

    }

    public class FooViewModel
    {
    } 

}