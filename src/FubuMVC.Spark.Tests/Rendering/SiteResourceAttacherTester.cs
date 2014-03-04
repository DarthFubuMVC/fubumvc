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
        private const string TestPath = "views/home/home.spark";

        private FubuSparkView _fubuSparkView;
        private ISparkViewEngine _engine;
        private IResourcePathManager _resourcePathManager;
        private SiteResourceAttacher.AppPath _appPath;

        protected override void beforeEach()
        {
            _fubuSparkView = MockFor<FubuSparkView>();
            _engine = MockFor<ISparkViewEngine>();
            _appPath = new SiteResourceAttacher.AppPath();
            MockFor<IFubuRequest>().Stub(x => x.Get<SiteResourceAttacher.AppPath>()).Return(_appPath);

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
            _appPath.ApplicationPath = applicationPath;

            _resourcePathManager
                .Stub(x => x.GetResourcePath(root, TestPath))
                .Return(expected);

            ClassUnderTest.Modify(_fubuSparkView);
            _fubuSparkView.SiteResource(TestPath).ShouldEqual(expected);
        }
    }
}