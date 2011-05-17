using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class OnceTableActivationTester : InteractionContext<OnceTableActivation>
    {
        private IFubuSparkView _view1;
        private IFubuSparkView _view2;

        protected override void beforeEach()
        {
            var views = Services.CreateMockArrayFor<IFubuSparkView>(2);
            _view1 = views[0];
            _view2 = views[1];
            _view1.Stub(x => x.OnceTable).PropertyBehavior();
            _view2.Stub(x => x.OnceTable).PropertyBehavior();
        }


        [Test]
        public void sets_the_same_instance_as_oncetable()
        {
            ClassUnderTest.Modify(_view1);
            ClassUnderTest.Modify(_view2);
            _view1.OnceTable.ShouldNotBeNull();
            _view1.OnceTable.ShouldBeTheSameAs(_view2.OnceTable);
        }
    }
}