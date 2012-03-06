//using System;
//using FubuMVC.Razor.RazorModel;
//using FubuTestingSupport;
//using NUnit.Framework;

//namespace FubuMVC.Razor.Tests.RazorModel
//{
//    [TestFixture]
//    public class ChunkLoaderTester : InteractionContext<ViewLoaderLocator>
//    {
//        private readonly IRazorTemplate _template1 = new Template("r/t1/path1", "r/t1", "t1");
//        private readonly IRazorTemplate _template2 = new Template("r/t2/path2", "r/t2", "t2");

//        private string _lastRequestedRoot;
//        private int _rootRequestCount;

//        protected override void beforeEach()
//        {
//            _rootRequestCount = 0;
//            _lastRequestedRoot = string.Empty;

//            Services.Inject<Func<string, IViewFolder>>(root =>
//            {
//                _lastRequestedRoot = root;
//                _rootRequestCount++;

//                return new InMemoryViewFolder
//                {
//                    {_template1.RelativePath(), @"<use master=""M1""/><div>path1</div>"},
//                    {_template2.RelativePath(), @"<use master=""M2""/><div>path2</div>"}
//                };
//            });
//        }

//        [Test]
//        public void view_folders_are_segregated_by_root_path()
//        {
//            ClassUnderTest.Locate(_template1);
//            _lastRequestedRoot.ShouldEqual(_template1.RootPath);
//            ClassUnderTest.Locate(_template2);
//            _lastRequestedRoot.ShouldEqual(_template2.RootPath);
//        }

//        [Test]
//        public void view_folders_are_cached_by_root_path()
//        {
//            ClassUnderTest.Locate(_template1);
//            ClassUnderTest.Locate(_template2);
//            ClassUnderTest.Locate(_template1);
//            _rootRequestCount.ShouldEqual(2);
//        }
//    }

//    [TestFixture]
//    public class ChunkLoaderExtensionsTester : InteractionContext<ViewLoaderLocator>
//    {
//        private readonly IRazorTemplate _razor1 = new Template("root/razor1", "root", "origin");
//        private readonly IRazorTemplate _razor2 = new Template("root/razor2", "root", "origin");
//        private readonly IRazorTemplate _razor3 = new Template("root/razor3", "root", "origin");

//        protected override void beforeEach()
//        {
//            Services.Inject<Func<string, IViewFolder>>(root => new InMemoryViewFolder
//            {
//                { _razor1.RelativePath(), @"<use master=""Fubu""/><div>Hail master Fubu..</div>" },
//                { _razor2.RelativePath(), @"<use namespace=""a.b.c""/><use namespace=""x.y.z""/><div>Namespaces</div>" },
//                { _razor3.RelativePath(), @"<use master=""""/><viewdata model=""Foo.Bar.Baz""><div>With Model - empty master</div>" }
//            });
//        }

//        [Test] 
//        public void when_razor_has_master_it_is_extracted()
//        {
//            ClassUnderTest.Locate(_razor1).Master().ShouldEqual("Fubu");
//        }

//        [Test]
//        public void when_razor_has_no_master_null_is_returned()
//        {
//            ClassUnderTest.Locate(_razor2).Master().ShouldBeNull();
//        }

//        [Test]
//        public void when_razor_has_empty_master_empty_string_is_returned()
//        {
//            ClassUnderTest.Locate(_razor3).Master().ShouldBeEmpty();
//        }

//        [Test]
//        public void when_razor_has_viewmodel_typename_is_returned()
//        {
//            ClassUnderTest.Locate(_razor3).ViewModel().ShouldEqual("Foo.Bar.Baz");
//        }

//        [Test]
//        public void when_razor_has_no_viewmodel_null_is_returned()
//        {
//            ClassUnderTest.Locate(_razor2).ViewModel().ShouldBeNull();
//        }

//        [Test]
//        public void when_razor_uses_namespaces_they_are_returned()
//        {
//            ClassUnderTest.Locate(_razor2).Namespaces()
//                .ShouldHaveTheSameElementsAs("a.b.c", "x.y.z");
//        }

//        [Test]
//        public void when_razor_uses_no_namespaces_empty_list_is_returned()
//        {
//            ClassUnderTest.Locate(_razor1).Namespaces().ShouldHaveCount(0);
//        }
//    }

//}