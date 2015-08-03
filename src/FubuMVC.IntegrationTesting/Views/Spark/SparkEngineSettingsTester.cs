using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Spark;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class SparkEngineSettingsTester : InteractionContext<SparkEngineSettings>
    {
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
            var faf = FubuApplicationFiles.ForDefault();

            ClassUnderTest.Search.AppendExclude("*A3.cshtml");
            ClassUnderTest.Search.AppendExclude("Templates/*.*");

            ClassUnderTest.Search.ExcludedFilesFor(faf.RootPath);
            ClassUnderTest.Search.IncludedFilesFor(faf.RootPath);

            var files = faf.FindFiles(ClassUnderTest.Search).ToArray();

            files.ShouldNotContain(f => f.Path.EndsWith("A3.cshtml"));
            files.ShouldNotContain(f => f.Path.EndsWith("A4.cshtml"));
        }

        [Test]
        public void do_not_precompile_when_in_development_mode()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.Mode = "development"))
            {
                runtime.Get<SparkEngineSettings>()
                    .PrecompileViews.ShouldBeFalse();
            }
            
        }

        [Test]
        public void explicitly_do_not_precompile_views()
        {
            using (var runtime = FubuRuntime.Basic(_ =>
            {
                _.Mode = "production";
                _.AlterSettings<SparkEngineSettings>(x => x.PrecompileViews = false);
            }))
            {
                runtime.Get<SparkEngineSettings>()
                    .PrecompileViews.ShouldBeFalse();
            }
        }
    }
}
