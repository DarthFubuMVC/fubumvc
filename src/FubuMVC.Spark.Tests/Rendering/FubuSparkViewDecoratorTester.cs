using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Urls;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class FubuSparkViewDecoratorTester : InteractionContext<FubuSparkViewDecorator>
    {
        private IFubuSparkView _view;

        protected override void beforeEach()
        {
            _view = MockFor<IFubuSparkView>();
            _view.Stub(x => x.Content).PropertyBehavior();
            _view.Stub(x => x.OnceTable).PropertyBehavior();
            _view.Stub(x => x.Output).PropertyBehavior();
            _view.Stub(x => x.Globals).PropertyBehavior();
            _view.Stub(x => x.ElementPrefix).PropertyBehavior();
            _view.Stub(x => x.GeneratedViewId).Return(Guid.NewGuid());
            _view.Stub(x => x.ServiceLocator).PropertyBehavior();
            _view.Stub(x => x.Urls).Return(MockFor<IUrlRegistry>());
            _view.Stub(x => x.CacheService).PropertyBehavior();
        }

        [Test]
        public void generatedviewid_is_forwarded_to_inner_view()
        {
            _view.GeneratedViewId.ShouldEqual(ClassUnderTest.GeneratedViewId);
        }

        [Test]
        public void content_is_forwarded_to_inner_view()
        {
            var content = new Dictionary<string, TextWriter> {{"zone1", new StringWriter()}};
            ClassUnderTest.Content = content;
            ClassUnderTest.Content
                .ShouldBeTheSameAs(_view.Content)
                .ShouldBeTheSameAs(content);
        }

        [Test]
        public void once_table_is_forwarded_to_inner_view()
        {
            var onceTable = new Dictionary<string, string> {{"once_key1", "once_value1"}};
            ClassUnderTest.OnceTable = onceTable;
            ClassUnderTest.OnceTable
                .ShouldBeTheSameAs(_view.OnceTable)
                .ShouldBeTheSameAs(onceTable);
        }

        [Test]
        public void globals_is_forwarded_to_inner_view()
        {
            var globals = new Dictionary<string, object> {{"global_key1", "global_value1"}};
            ClassUnderTest.Globals = globals;
            ClassUnderTest.Globals
                .ShouldBeTheSameAs(_view.Globals)
                .ShouldBeTheSameAs(globals);
        }

        [Test]
        public void output_is_forwarded_to_inner_view()
        {
            var output = new StringWriter();
            ClassUnderTest.Output = output;
            ClassUnderTest.Output
                .ShouldBeTheSameAs(_view.Output)
                .ShouldBeTheSameAs(output);
        }

        [Test]
        public void elementprefix_is_forwarded_to_inner_view()
        {
            const string elementPrefix = "fubu";
            ClassUnderTest.ElementPrefix = elementPrefix;
            ClassUnderTest.ElementPrefix
                .ShouldBeTheSameAs(_view.ElementPrefix)
                .ShouldBeTheSameAs(elementPrefix);
        }

        [Test]
        public void servicelocator_is_forwarded_to_inner_view()
        {
            var serviceLocator = MockFor<IServiceLocator>();
            ClassUnderTest.ServiceLocator = serviceLocator;
            ClassUnderTest.ServiceLocator
                .ShouldBeTheSameAs(_view.ServiceLocator)
                .ShouldBeTheSameAs(serviceLocator);
        }

        [Test]
        public void urls_is_forwarded_to_inner_view()
        {
            ClassUnderTest.Urls.ShouldBeTheSameAs(_view.Urls);
        }

        [Test]
        public void get_is_forwarded_to_inner_view()
        {
            var guid = Guid.NewGuid();
            _view.Stub(x => x.Get<string>()).Return("Hello World");
            _view.Stub(x => x.Get<Guid>()).Return(guid);
            ClassUnderTest.Get<string>().ShouldEqual("Hello World");
            ClassUnderTest.Get<Guid>().ShouldEqual(guid);
        }

        [Test]
        public void getnew_is_forwarded_to_inner_view()
        {
            var guid = Guid.NewGuid();
            _view.Stub(x => x.GetNew<string>()).Return("Hello World");
            _view.Stub(x => x.GetNew<Guid>()).Return(guid);
            ClassUnderTest.GetNew<string>().ShouldEqual("Hello World");
            ClassUnderTest.GetNew<Guid>().ShouldEqual(guid);
        }

        [Test]
        public void render_is_forwarded_to_inner_view()
        {
            _view.Expect(x => x.Render(MockFor<IFubuRequestContext>()));
            ClassUnderTest.Render(MockFor<IFubuRequestContext>());
            _view.VerifyAllExpectations();
        }

        [Test]
        public void render_wraps_execution_with_pre_and_post_render_delegates()
        {
            var callStack = new List<string>();

            ClassUnderTest.PreRender += x => callStack.Add("Pre Render1");
            ClassUnderTest.PreRender += obj => callStack.Add("Pre Render2");
            ClassUnderTest.PostRender += x => callStack.Add("Post Render1");
            ClassUnderTest.PostRender += x => callStack.Add("Post Render2");
            _view.Stub(x => x.Render(MockFor<IFubuRequestContext>())).WhenCalled(x => callStack.Add("Render View"));

            ClassUnderTest.Render(MockFor<IFubuRequestContext>());

            callStack.ShouldHaveCount(5)
                .Join("|")
                .ShouldEqual("Pre Render1|Pre Render2|Render View|Post Render1|Post Render2");
        }

        [Test]
        public void cache_service_is_forwarded_to_inner_view()
        {
            ClassUnderTest.CacheService = MockFor<ICacheService>();
            ClassUnderTest.CacheService.ShouldEqual(MockFor<ICacheService>()).ShouldEqual(_view.CacheService);
        }
    }
}