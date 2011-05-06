using System.IO;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class TemplateExtensionsTester
    {
        private readonly SparkItem _bottomItem;
        private readonly SparkItem _middleItem;
        private readonly SparkItem _topItem;

        public TemplateExtensionsTester()
        {
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

            var bottomPath = Path.Combine(rootPath, "Finding", "Sherlock", "Homes.spark");
            var middlePath = Path.Combine(rootPath, "Dining", "Philosophers.spark");
            var topPath = Path.Combine(rootPath, "Livelock.spark");
            
            _bottomItem = new SparkItem(bottomPath, rootPath, "chuck");
            _middleItem = new SparkItem(middlePath, rootPath, "chuck");
            _topItem = new SparkItem(topPath, rootPath, "chuck");
        }

        [Test]
        public void relative_path_returns_correct_fragment_1()
        {
            _bottomItem.RelativePath()
                .ShouldEqual("Finding{0}Sherlock{0}Homes.spark".ToFormat(Path.DirectorySeparatorChar));
        }

        [Test]
        public void relative_path_returns_correct_fragment_2()
        {
            _middleItem.RelativePath()
                .ShouldEqual("Dining{0}Philosophers.spark".ToFormat(Path.DirectorySeparatorChar));
        }

        [Test]
        public void relative_path_returns_correct_fragment_3()
        {
            _topItem.RelativePath().ShouldEqual("Livelock.spark");
        }

        [Test]
        public void name_returns_filename_without_extension()
        {
            _topItem.Name().ShouldEqual("Livelock");
        }

        [Test]
        public void directory_path_returns_directory_path_of_file()
        {
            _topItem.DirectoryPath().ShouldEqual(_topItem.RootPath);
        }
		
		[Test]
        public void is_partial_returns_true_if_file_starts_with_underscore_and_ends_with_dot_spark()
        {
			new SparkItem("_Partial.spark", "", "").IsPartial().ShouldBeTrue();
			new SparkItem("_Template.htm", "", "").IsPartial().ShouldBeFalse();
        }
		
		[Test]
        public void is_spark_view_returns_true_if_file_ends_with_dot_spark()
        {
			_bottomItem.IsSparkView().ShouldBeTrue();
			new SparkItem("bindings.xml", "", "").IsSparkView().ShouldBeFalse();
        }
		
		[Test]
        public void is_xml_returns_true_if_file_ends_with_xml()
        {
			new SparkItem("bindings.xml", "", "").IsXml().ShouldBeTrue();
        }
    }
}
