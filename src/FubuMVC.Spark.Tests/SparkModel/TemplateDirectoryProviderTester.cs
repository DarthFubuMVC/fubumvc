using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class TemplateDirectoryProviderTester : InteractionContext<TemplateDirectoryProvider>
    {
        private string _root;
        private ISharedPathBuilder _builder;
        private Template _item;
        private Template _packageItem;
        private IList<Template> _items;
        protected override void beforeEach()
        {
            _root = AppDomain.CurrentDomain.BaseDirectory;

            _item = new Template(getPath("Actions", "Home", "home.spark"), _root, FubuSparkConstants.HostOrigin);
            _packageItem = new Template(getPath("Packages", "Package1", "Actions", "Home", "home.spark"), _root, "Package1");

            _items = new List<Template> { _item, _packageItem };

            _builder = MockFor<ISharedPathBuilder>();
            _builder.Stub(x => x.SharedFolderNames).Return(new[] { Constants.Shared });
            Services.Inject(_builder);
        }

        private string getPath(params string[] parts)
        {
            return Path.Combine(_root, Path.Combine(parts));
        }

        [Test]
        public void the_shared_path_builder_returns_the_directories_from_the_item_root_and_file_path()
        {
            var paths = Enumerable.Empty<string>();
            _builder
                .Expect(x => x.BuildFrom(_item.FilePath, _item.RootPath, false))
                .Return(paths);

            ClassUnderTest.SharedPathsOf(_item, _items).ToList();
            _builder.VerifyAllExpectations();
        }

        [Test]
        public void output_contains_the_items_returned_by_the_shared_path_builder()
        {
            var paths = new[] { getPath("Shared"), getPath("Views", "Shared") };
            _builder
                .Stub(x => x.BuildFrom(null, null, false)).IgnoreArguments()
                .Return(paths);

            ClassUnderTest.SharedPathsOf(_item, _items)
                .Each(x => paths.ShouldContain(x));
        }

        [Test]
        public void when_the_item_origin_is_host_output_equals_shared_path_builder()
        {
            var paths = new[] { getPath("Shared"), getPath("Views", "Shared") };
            _builder
                .Stub(x => x.BuildFrom(null, null, false)).IgnoreArguments()
                .Return(paths);

            ClassUnderTest.SharedPathsOf(_item, _items)
                .SequenceEqual(paths).ShouldBeTrue();
        }

        [Test]
        public void when_the_item_origin_is_not_host_but_no_host_item_exists_output_equals_shared_path_builder()
        {
            var paths = new[] { getPath("Shared"), 
                getPath("Views", "Shared"),
                getPath("Packages", "Package1", "Shared"), 
                getPath("Packages", "Package1", "Actions", "Shared") };
            _builder
                .Stub(x => x.BuildFrom(null, null, false)).IgnoreArguments()
                .Return(paths);

            _items.Remove(_item);

            ClassUnderTest.SharedPathsOf(_packageItem, _items)
                .SequenceEqual(paths).ShouldBeTrue();
        }

        [Test]
        public void when_the_item_origin_is_not_host_output_contains_shared_folders_from_host()
        {
            var rootShared = getPath("Shared");
            var paths = new[] { getPath("Views", "Shared"),
                getPath("Packages", "Package1", "Shared"), 
                getPath("Packages", "Package1", "Actions", "Shared") };
            _builder
                .Stub(x => x.BuildFrom(null, null, false))
                .IgnoreArguments().Return(paths);

            ClassUnderTest.SharedPathsOf(_packageItem, _items)
                .SequenceEqual(paths.Union(new[] { rootShared }))
                .ShouldBeTrue();
        }
    }
}