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
		private string _testPath = "views/home/home.spark";
		
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
        public void if_view_is_not_ifubusparkview_returns_false()
        {
            ClassUnderTest.Applies(_sparkView).ShouldBeFalse();
        }

        [Test]
        public void if_view_is_ifubusparkview_returns_true()
        {
            ClassUnderTest.Applies(_fubuSparkView).ShouldBeTrue();
        }
		
		[Test]
		public void resolves_correctly_with_empty_app_path()
		{
			after_modification_site_resource_resolves_correctly("", "", _testPath);
		}

		[Test]
		public void resolves_correctly_with_slashed_app_path()
		{
			after_modification_site_resource_resolves_correctly("/", "", _testPath);
		}		

		[Test]
		public void resolves_correctly_with_before_slash_on_named_app_path()
		{
			after_modification_site_resource_resolves_correctly("/Fubu", "/Fubu", "/Fubu/{0}".ToFormat(_testPath));
		}

		[Test]
		public void resolves_correctly_with_after_slash_on_named_app_path()
		{
			after_modification_site_resource_resolves_correctly("Fubu/", "/Fubu", "/Fubu/{0}".ToFormat(_testPath));
		}

		[Test]
		public void resolves_correctly_with_wrapped_slashes_on_named_app_path()
		{
			after_modification_site_resource_resolves_correctly("/Fubu/", "/Fubu", "/Fubu/{0}".ToFormat(_testPath));
		}
		
		private void after_modification_site_resource_resolves_correctly(string applicationPath, string root, string expected)
        {
            _request
                .Stub(x => x.Get<SiteResourceAttacher.AppPath>())
                .Return(new SiteResourceAttacher.AppPath {ApplicationPath = applicationPath});

            _resourcePathManager
                .Stub(x => x.GetResourcePath(root, _testPath))
                .Return(expected);

            ClassUnderTest.Modify(_fubuSparkView);

            _fubuSparkView.SiteResource(_testPath).ShouldEqual(expected);
        }
    }
}