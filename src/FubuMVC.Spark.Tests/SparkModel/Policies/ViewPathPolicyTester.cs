using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Policies
{
    [TestFixture]
    public class ViewPathPolicyTester : InteractionContext<ViewPathPolicy>
    {
        private ITemplate _template;
        private string _origin;
        protected override void beforeEach()
        {
            _origin = FubuSparkConstants.HostOrigin;
            _template = MockFor<ITemplate>();
            _template.Stub(x => x.Origin).Return(null).WhenCalled(x => x.ReturnValue = _origin);
            _template.Stub(x => x.RootPath).Return("root");
            _template.Stub(x => x.FilePath).Return(FileSystem.Combine("root", "view.spark"));
            _template.Stub(x => x.ViewPath).PropertyBehavior();
        }

        [Test]
        public void when_origin_is_host_viewpath_is_not_prefixed()
        {
            ClassUnderTest.Apply(_template);
            _template.ViewPath.ShouldEqual("view.spark");
        }

        [Test]
        public void when_origin_is_not_host_view_path_is_prefixed()
        {
            _origin = "Foo";
            ClassUnderTest.Apply(_template);
            _template.ViewPath.ShouldEqual(FileSystem.Combine("_" + _origin, "view.spark"));
        }

		[Test]
        public void it_matches_when_viewpath_is_empty()
		{
		    _template.ViewPath = null;
            ClassUnderTest.Matches(_template).ShouldBeTrue();
        }

        [Test]
        public void it_does_not_matches_when_viewpath_is_not_empty()
        {
            _template.ViewPath = "someone/else/did/this";
            ClassUnderTest.Matches(_template).ShouldBeFalse();
        }
    }
}