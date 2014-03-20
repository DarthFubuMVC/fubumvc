using FubuCore;
using FubuMVC.Core.Assets;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetSettingsTester
    {
        
    }

    [TestFixture]
    public class when_creating_the_default_search
    {
        private FileSet search = new AssetSettings().CreateAssetSearch();

        [Test]
        public void exclude_should_be_null()
        {
            search.Exclude.ShouldBeNull();
        }

        [Test]
        public void should_be_deep()
        {
            search.DeepSearch.ShouldBeTrue();
        }

        [Test]
        public void should_look_for_js_files()
        {
            search.Include.ShouldContain("*.js");
        }

        [Test]
        public void should_look_for_css_files()
        {
            search.Include.ShouldContain("*.css");
        }

        [Test]
        public void should_include_all_file_types_registered_as_images()
        {
            search.Include.ShouldContain("*.bmp");
            search.Include.ShouldContain("*.png");
            search.Include.ShouldContain("*.gif");
            search.Include.ShouldContain("*.jpg");
            search.Include.ShouldContain("*.jpeg");
        }
    }
}