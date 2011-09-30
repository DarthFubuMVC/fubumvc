using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;
using FubuMVC.Core;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class SiteResourceAttacherTester : InteractionContext<SiteResourceAttacher>
    {
		private const string TestPath = "views/home/home.spark";
		
        private IFubuSparkView _sparkView;
        private FubuSparkView _fubuSparkView;
        private ISparkViewEngine _engine;
        private CurrentRequest _request;
        private IResourcePathManager _resourcePathManager;

        protected override void beforeEach()
        {
            _sparkView = MockFor<FubuSparkView>();
			_fubuSparkView = MockFor<FubuSparkView>();
            _engine = MockFor<ISparkViewEngine>();
            _request = MockFor<CurrentRequest>();
            MockFor<IFubuRequest>().Stub(x => x.Get<CurrentRequest>()).Return(_request);

            _resourcePathManager = MockFor<IResourcePathManager>();

            _engine.Stub(x => x.ResourcePathManager).Return(_resourcePathManager);
        }
		
		[Test]
		public void resolves_correctly_with_empty_app_path()
		{
			after_modification_site_resource_resolves_correctly("", "", TestPath);
		}

		[Test]
		public void resolves_correctly_with_slashed_app_path()
		{
			after_modification_site_resource_resolves_correctly("/", "", TestPath);
		}		

		[Test]
		public void resolves_correctly_with_before_slash_on_named_app_path()
		{
			after_modification_site_resource_resolves_correctly("/Fubu", "/Fubu", "/Fubu/{0}".ToFormat(TestPath));
		}

		[Test]
		public void resolves_correctly_with_after_slash_on_named_app_path()
		{
			after_modification_site_resource_resolves_correctly("Fubu/", "/Fubu", "/Fubu/{0}".ToFormat(TestPath));
		}

		[Test]
		public void resolves_correctly_with_wrapped_slashes_on_named_app_path()
		{
			after_modification_site_resource_resolves_correctly("/Fubu/", "/Fubu", "/Fubu/{0}".ToFormat(TestPath));
		}
		
		private void after_modification_site_resource_resolves_correctly(string applicationPath, string root, string expected)
        {
            _request.ApplicationPath = applicationPath;
            _resourcePathManager
                .Stub(x => x.GetResourcePath(root, TestPath))
                .Return(expected);

            ClassUnderTest.Modify(_fubuSparkView);

            _fubuSparkView.SiteResource(TestPath).ShouldEqual(expected);
        }
    }
}