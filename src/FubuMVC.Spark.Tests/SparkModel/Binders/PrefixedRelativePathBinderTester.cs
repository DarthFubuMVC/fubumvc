using System;
using System.IO;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binders
{
    [TestFixture]
    public class PrefixedRelativePathBinderTester : InteractionContext<PrefixedRelativePathBinder>
    {
        private  SparkItem _item;

        protected override void beforeEach()
        {
            var rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App");
            var filePath = Path.Combine(rootPath, "Views", "Home.spark");
            _item = new SparkItem(filePath, rootPath, "") { PathPrefix = "_Prefix_" };
            ClassUnderTest.Bind(_item, null);
        }

        [Test]
        public void prefixed_relative_path_is_the_path_prefix_plus_relative_path()
        {
            _item.PrefixedRelativePath.ShouldEqual("_Prefix_\\Views\\Home.spark");
        }
    }
}