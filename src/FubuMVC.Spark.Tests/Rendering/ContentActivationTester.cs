using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ContentActivationTester : InteractionContext<ContentActivation>
    {
        private IFubuSparkView _view1;
        private IFubuSparkView _view2;

        protected override void beforeEach()
        {
            var views = Services.CreateMockArrayFor<IFubuSparkView>(2);
            _view1 = views[0];
            _view2 = views[1];
            _view1.Stub(x => x.Content).PropertyBehavior();
            _view2.Stub(x => x.Content).PropertyBehavior();
        }


        [Test]
        public void sets_the_same_instance_as_content()
        {
            ClassUnderTest.Modify(_view1);
            ClassUnderTest.Modify(_view2);
            _view1.Content.ShouldNotBeNull();
            _view1.Content.ShouldBeTheSameAs(_view2.Content);
        }
    }
}