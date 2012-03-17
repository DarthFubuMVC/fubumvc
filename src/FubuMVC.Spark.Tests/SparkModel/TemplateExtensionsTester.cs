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
        private readonly ITemplate _bottomTemplate;
        private readonly ITemplate _middleTemplate;
        private readonly ITemplate _topTemplate;

        public TemplateExtensionsTester()
        {
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

            var bottomPath = Path.Combine(rootPath, "Finding", "Sherlock", "Homes.spark");
            var middlePath = Path.Combine(rootPath, "Dining", "Philosophers.spark");
            var topPath = Path.Combine(rootPath, "Livelock.spark");
            
            _bottomTemplate = new Template(bottomPath, rootPath, "chuck");
            _middleTemplate = new Template(middlePath, rootPath, "chuck");
            _topTemplate = new Template(topPath, rootPath, "chuck");
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
			new Template("_Partial.spark", "", "").IsPartial().ShouldBeTrue();
        }
		
		[Test]
        public void is_spark_view_returns_true_if_file_ends_with_dot_spark()
        {
			_bottomTemplate.IsSparkView().ShouldBeTrue();
			new Template("bindings.xml", "", "").IsSparkView().ShouldBeFalse();
        }
		
		[Test]
        public void is_xml_returns_true_if_file_ends_with_xml()
        {
			new Template("bindings.xml", "", "").IsXml().ShouldBeTrue();
        }
    }
}
