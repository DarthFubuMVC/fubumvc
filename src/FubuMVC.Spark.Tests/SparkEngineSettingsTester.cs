using System.Linq;

using FubuMVC.Core;
using FubuMVC.Core.Runtime.Files;

using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class SparkEngineSettingsTester : InteractionContext<SparkEngineSettings>
    {
        [TearDown]
        public void TearDown()
        {
            FubuMode.Reset();
        }

        [Test]
        public void includes_spark_views_and_bindings_by_default()
        {
            ClassUnderTest.Search.Include.Split(';')
                .ShouldHaveTheSameElementsAs("*.spark", "*.shade", "bindings.xml");
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

            ClassUnderTest.Search.ExcludedFilesFor(faf.GetApplicationPath());
            ClassUnderTest.Search.IncludedFilesFor(faf.GetApplicationPath());

            var files = faf.FindFiles(ClassUnderTest.Search).ToArray();

            files.ShouldNotHave(f => f.Path.EndsWith("A3.cshtml"));
            files.ShouldNotHave(f => f.Path.EndsWith("A4.cshtml"));
        }

        [Test]
        public void do_not_precompile_when_in_development_mode()
        {
            FubuMode.Mode(FubuMode.Development);
            ClassUnderTest.PrecompileViews.ShouldBeFalse();
            
        }

        [Test]
        public void precompile_views_not_in_development_mode()
        {
            FubuMode.Mode("Production");
            ClassUnderTest.PrecompileViews.ShouldBeTrue();
        }

        [Test]
        public void explicitly_do_not_precompile_views()
        {
            ClassUnderTest.PrecompileViews = false;
            FubuMode.Mode("Production");

            ClassUnderTest.PrecompileViews.ShouldBeFalse();
        }
    }
}
