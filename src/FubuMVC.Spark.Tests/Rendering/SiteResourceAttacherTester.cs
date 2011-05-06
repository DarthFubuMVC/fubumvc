using System;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class SiteResourceAttacherTester : InteractionContext<SiteResourceAttacher>
    {
        private ISparkView _sparkView;
        private FubuSparkView _fubuSparkView;
        private ISparkViewEngine _engine;
        private IFubuRequest _request;
        private IResourcePathManager _resourcePathManager;
        protected override void beforeEach()
        {
            _sparkView = MockFor<ISparkView>();
            _fubuSparkView = MockFor<FubuSparkView>();
            _engine = MockFor<ISparkViewEngine>();
            _request = MockFor<IFubuRequest>();
            _resourcePathManager = MockFor<IResourcePathManager>();

            _engine.Stub(x => x.ResourcePathManager).Return(_resourcePathManager);
            Services.Inject(_engine);
            Services.Inject(_request);
        }
        [Test]
        public void if_view_is_not_ifubupage_returns_false()
        {
            ClassUnderTest.Applies(_sparkView).ShouldBeFalse();
        }

        [Test]
        public void if_view_is_ifubusparkview_returns_true()
        {
            ClassUnderTest.Applies(_fubuSparkView).ShouldBeTrue();
        }

        [Test]
        [TestCase("", "", "views/home/home.spark", "views/home/home.spark")]
        [TestCase("/", "", "views/home/home.spark", "views/home/home.spark")]
        [TestCase("/Fubu", "/Fubu", "views/home/home.spark", "/Fubu/views/home/home.spark")]
        [TestCase("Fubu/", "/Fubu", "views/home/home.spark", "/Fubu/views/home/home.spark")]
        [TestCase("/Fubu/", "/Fubu", "views/home/home.spark", "/Fubu/views/home/home.spark")]
        public void after_modifying_the_view_site_resource_is_resolved_correctly(string applicationPath, string root, string path, string expected)
        {
            _request
                .Stub(x => x.Get<SiteResourceAttacher.AppPath>())
                .Return(new SiteResourceAttacher.AppPath {ApplicationPath = applicationPath});

            _resourcePathManager
                .Stub(x => x.GetResourcePath(root, path))
                .Return(expected);

            ClassUnderTest.Modify(_fubuSparkView);

            var resourcePath = _fubuSparkView.SiteResource(path);
            resourcePath.ShouldEqual(expected);
        }
    }
}