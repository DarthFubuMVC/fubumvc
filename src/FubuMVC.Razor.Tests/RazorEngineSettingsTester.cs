using FubuMVC.Core.Runtime.Files;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests
{
	[TestFixture]
    public class RazorEngineSettingsTester : InteractionContext<RazorEngineSettings>
    {
		[Test]
	    public void includes_razor_views_by_default()
	    {
            ClassUnderTest.Search.Include.Split(';')
                .ShouldHaveTheSameElementsAs("*cshtml", "*vbhtml");
	    }

        [Test]
        public void uses_deep_search_by_default()
        {
            ClassUnderTest.Search.DeepSearch.ShouldBeTrue();
        }

        [Test]
        public void excludes_bin_and_obj_by_default()
        {
            ClassUnderTest.Search.Exclude.Split(';')
                .ShouldHaveTheSameElementsAs("bin/*.*", "obj/*.*");
        }

	    [Test]
        public void ignores_excluded_folders()
        {
            var faf = new FubuApplicationFiles();

            ClassUnderTest.Search.AppendExclude("*A3.cshtml");
            ClassUnderTest.Search.AppendExclude("Templates/*.*");

            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            var ex = ClassUnderTest.Search.ExcludedFilesFor(faf.GetApplicationPath());
            var inc = ClassUnderTest.Search.IncludedFilesFor(faf.GetApplicationPath());

            var files = faf.FindFiles(ClassUnderTest.Search);

            files.ShouldNotHave(f => f.Path.EndsWith("A3.cshtml"));
            files.ShouldNotHave(f => f.Path.EndsWith("A4.cshtml"));
        }

        [Test]
        public void default_page_base_type_is_fuburazorview()
        {
            ClassUnderTest.BaseTemplateType.ShouldEqual(typeof(FubuRazorView));
        }
    }
}
