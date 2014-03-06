using System.IO;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests.RazorModel
{
    [TestFixture]
    public class TemplateExtensionsTester
    {
        private readonly RazorTemplate _bottomTemplate;
        private readonly RazorTemplate _middleTemplate;
        private readonly RazorTemplate _topTemplate;

        public TemplateExtensionsTester()
        {
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

            var bottomPath = Path.Combine(rootPath, "Finding", "Sherlock", "Homes.cshtml");
            var middlePath = Path.Combine(rootPath, "Dining", "Philosophers.cshtml");
            var topPath = Path.Combine(rootPath, "Livelock.cshtml");
            Assert.Fail("Redo.  Maybe just in TemplateTester");
//            _bottomTemplate = new RazorTemplate(bottomPath, rootPath, "chuck");
//            _middleTemplate = new RazorTemplate(middlePath, rootPath, "chuck");
//            _topTemplate = new RazorTemplate(topPath, rootPath, "chuck");
        }

        [Test]
        public void relative_path_returns_correct_fragment_1()
        {
            _bottomTemplate.RelativePath()
                .ShouldEqual("Finding{0}Sherlock{0}Homes.cshtml".ToFormat(Path.DirectorySeparatorChar));
        }

        [Test]
        public void relative_path_returns_correct_fragment_2()
        {
            _middleTemplate.RelativePath()
                .ShouldEqual("Dining{0}Philosophers.cshtml".ToFormat(Path.DirectorySeparatorChar));
        }

        [Test]
        public void relative_path_returns_correct_fragment_3()
        {
            _topTemplate.RelativePath().ShouldEqual("Livelock.cshtml");
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
        public void is_razor_view_returns_true_if_file_ends_with_dot_razor()
        {
			_bottomTemplate.IsRazorView().ShouldBeTrue();
			
            Assert.Fail("Redo.");
            
            //new RazorTemplate("bindings.xml", "", "").IsRazorView().ShouldBeFalse();
        }
    }
}
