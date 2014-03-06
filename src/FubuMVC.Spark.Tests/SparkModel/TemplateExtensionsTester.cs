using System.IO;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class TemplateExtensionsTester
    {
        private readonly ISparkTemplate _bottomTemplate;
        private readonly ISparkTemplate _middleTemplate;
        private readonly ISparkTemplate _topTemplate;

        public TemplateExtensionsTester()
        {
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

            var bottomPath = Path.Combine(rootPath, "Finding", "Sherlock", "Homes.spark");
            var middlePath = Path.Combine(rootPath, "Dining", "Philosophers.spark");
            var topPath = Path.Combine(rootPath, "Livelock.spark");
            
            _bottomTemplate = new SparkTemplate(bottomPath, rootPath, "chuck");
            _middleTemplate = new SparkTemplate(middlePath, rootPath, "chuck");
            _topTemplate = new SparkTemplate(topPath, rootPath, "chuck");
        }

        [Test]
        public void relative_path_returns_correct_fragment_1()
        {
            _bottomTemplate.RelativePath()
                .ShouldEqual("Finding{0}Sherlock{0}Homes.spark".ToFormat(Path.DirectorySeparatorChar));
        }

        [Test]
        public void relative_path_returns_correct_fragment_2()
        {
            _middleTemplate.RelativePath()
                .ShouldEqual("Dining{0}Philosophers.spark".ToFormat(Path.DirectorySeparatorChar));
        }

        [Test]
        public void relative_path_returns_correct_fragment_3()
        {
            _topTemplate.RelativePath().ShouldEqual("Livelock.spark");
        }

        [Test]
        public void name_returns_filename_without_extension()
        {
            _topTemplate.Name().ShouldEqual("Livelock");
        }

        [Test]
        public void directory_path_returns_directory_path_of_file()
        {
            _topTemplate.DirectoryPath().ShouldEqual(_topTemplate.RootPath);
        }
		
		[Test]
        public void is_partial_returns_true_if_file_starts_with_underscore_and_ends_with_dot_spark()
        {
			new SparkTemplate("_Partial.spark", "", "").IsPartial().ShouldBeTrue();
        }
		
		[Test]
        public void is_spark_view_returns_true_if_file_ends_with_dot_spark()
        {
			_bottomTemplate.IsSparkView().ShouldBeTrue();
			new SparkTemplate("bindings.xml", "", "").IsSparkView().ShouldBeFalse();
        }
		
		[Test]
        public void is_xml_returns_true_if_file_ends_with_xml()
        {
			new SparkTemplate("bindings.xml", "", "").IsXml().ShouldBeTrue();
        }
    }
}
