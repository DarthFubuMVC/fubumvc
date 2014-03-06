using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class FubuPartialProviderTester : InteractionContext<FubuPartialProvider>
    {
        private ITemplateDirectoryProvider<ISparkTemplate> _templateDirectoryProvider;
        private string _viewPath;
        private List<string> _sharedTemplates;

        protected override void beforeEach()
        {
            _viewPath = FileSystem.Combine("_Package", "Handlers", "Models", "test.spark");// @"_Package\Handlers\Models\test.spark");
            _templateDirectoryProvider = MockFor<ITemplateDirectoryProvider<ISparkTemplate>>();
            _sharedTemplates = new List<string>();
            _templateDirectoryProvider
                .Expect(x => x.SharedViewPathsForOrigin("Package"))
                .Return(_sharedTemplates);
        }

        [Test]
        public void get_paths_includes_default_partial_paths()
        {
            var paths = ClassUnderTest.GetPaths(_viewPath);
            paths.ShouldHaveTheSameElementsAs(new[]
            {
                FileSystem.Combine("_Package", "Handlers", "Models"),
                FileSystem.Combine("_Package", "Handlers", "Models", "Shared"),
                FileSystem.Combine("_Package", "Handlers"),
                FileSystem.Combine("_Package", "Handlers", "Shared"),
                @"_Package",
                FileSystem.Combine("_Package", "Shared"),
                @"",
                @"Shared",
            });
        }

        [Test]
        public void get_paths_also_includes_any_shared_view_paths_for_origin()
        {
            string sharedTemplate = FileSystem.Combine("_Global", "Shared");
            _sharedTemplates.Add(sharedTemplate);
            var paths = ClassUnderTest.GetPaths(_viewPath);
            paths.ShouldContain(sharedTemplate);
        }

        [Test]
        public void can_parse_origin_from_view_path()
        {
            var inputs = new[]
            {
                @"_Package",
                @"_Package\Shared",
                @"_Package\Shared\Folder",
                @"_Package/Shared", //Mono path
                @"Package\",
                @"_Package\",
            };

            inputs.Each(x => x.GetOrigin().ShouldEqual("Package"));
        }
    }
}