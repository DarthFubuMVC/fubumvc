using System;
using System.Linq;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Spark.Compiler;
using Spark.FileSystem;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class ChunkLoaderTester : InteractionContext<ChunkLoader>
    {
        private readonly Template _item1 = new Template("r/t1/path1", "r/t1", "t1");
        private readonly Template _item2 = new Template("r/t2/path2", "r/t2", "t2");
        private string _lastRequestedRoot;
        private int _rootRequestCount;

        protected override void beforeEach()
        {
            _rootRequestCount = 0;
            _lastRequestedRoot = string.Empty;

            Services.Inject<Func<string, IViewFolder>>(root =>
            {
                _lastRequestedRoot = root;
                _rootRequestCount++;

                return new InMemoryViewFolder
                {
                    {_item1.RelativePath(), @"<use master=""M1""/><div>path1</div>"},
                    {_item2.RelativePath(), @"<use master=""M2""/><div>path2</div>"}
                };
            });
        }

        [Test]
        public void view_folders_are_segregated_by_root_path()
        {
            ClassUnderTest.Load(_item1);
            _lastRequestedRoot.ShouldEqual(_item1.RootPath);
            ClassUnderTest.Load(_item2);
            _lastRequestedRoot.ShouldEqual(_item2.RootPath);
        }

        [Test]
        public void view_folders_are_cached_by_root_path()
        {
            ClassUnderTest.Load(_item1);
            ClassUnderTest.Load(_item2);
            ClassUnderTest.Load(_item1);
            _rootRequestCount.ShouldEqual(2);
        }
    }

    [TestFixture]
    public class ChunkLoaderExtensionsTester : InteractionContext<ChunkLoader>
    {
        private readonly Template _spark1 = new Template("root/spark1", "root", "origin");
        private readonly Template _spark2 = new Template("root/spark2", "root", "origin");
        private readonly Template _spark3 = new Template("root/spark3", "root", "origin");

        protected override void beforeEach()
        {
            Services.Inject<Func<string, IViewFolder>>(root =>
            {
                return new InMemoryViewFolder
                {
                    { _spark1.RelativePath(), @"<use master=""Fubu""/><div>Hail master Fubu..</div>" },
                    { _spark2.RelativePath(), @"<use namespace=""a.b.c""/><use namespace=""x.y.z""/><div>Namespaces</div>" },
                    { _spark3.RelativePath(), @"<use master=""""/><viewdata model=""Foo.Bar.Baz""><div>With Model - empty master</div>" }
                };
            });
        }

        [Test] 
        public void when_spark_has_master_it_is_extracted()
        {
            ClassUnderTest.Load(_spark1).Master().ShouldEqual("Fubu");
        }

        [Test]
        public void when_spark_has_no_master_null_is_returned()
        {
            ClassUnderTest.Load(_spark2).Master().ShouldBeNull();
        }

        [Test]
        public void when_spark_has_empty_master_empty_string_is_returned()
        {
            ClassUnderTest.Load(_spark3).Master().ShouldBeEmpty();
        }

        [Test]
        public void when_spark_has_viewmodel_typename_is_returned()
        {
            ClassUnderTest.Load(_spark3).ViewModel().ShouldEqual("Foo.Bar.Baz");
        }

        [Test]
        public void when_spark_has_no_viewmodel_null_is_returned()
        {
            ClassUnderTest.Load(_spark2).ViewModel().ShouldBeNull();
        }

        [Test]
        public void when_spark_uses_namespaces_they_are_returned()
        {
            ClassUnderTest.Load(_spark2).Namespaces()
                .ShouldHaveTheSameElementsAs("a.b.c", "x.y.z");
        }

        [Test]
        public void when_spark_uses_no_namespaces_empty_list_is_returned()
        {
            ClassUnderTest.Load(_spark1).Namespaces().ShouldHaveCount(0);
        }
    }
}