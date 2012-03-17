using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ViewModifierServiceTester : InteractionContext<ViewModifierService<IFubuSparkView>>
    {
        private FubuSparkView _sparkView1;
        private FubuSparkView _sparkView2;
        private FubuSparkView _sparkView3;
        private IViewModifier<IFubuSparkView> _modification1;
        private IViewModifier<IFubuSparkView> _modification2;
        private IViewModifier<IFubuSparkView> _modification3;

        protected override void beforeEach()
        {
            var modifications = Services.CreateMockArrayFor<IViewModifier<IFubuSparkView>>(3);
            _sparkView1 = MockRepository.GenerateMock<FubuSparkView>();
            _sparkView2 = MockRepository.GenerateMock<FubuSparkView>();
            _sparkView3 = MockRepository.GenerateMock<FubuSparkView>();
            _modification1 = modifications[0];
            _modification2 = modifications[1];
            _modification3 = modifications[2];

            _modification1.Expect(x => x.Applies(_sparkView1)).Return(true);
            _modification2.Expect(x => x.Applies(_sparkView2)).Return(false);
            _modification3.Expect(x => x.Applies(_sparkView2)).Return(true);

            _modification1.Expect(x => x.Modify(_sparkView1)).Return(_sparkView2);
            _modification2.Expect(x => x.Modify(Arg<IFubuSparkView>.Is.Anything)).Repeat.Never();
            _modification3.Expect(x => x.Modify(_sparkView2)).Return(_sparkView3);
        }

        [Test]
        public void only_applicable_modifications_are_used()
        {
            ClassUnderTest.Modify(_sparkView1);
            _modification1.VerifyAllExpectations();
            _modification2.VerifyAllExpectations();
            _modification3.VerifyAllExpectations();
        }

        [Test]
        public void the_resulting_view_comes_from_the_modifiers()
        {
            ClassUnderTest.Modify(_sparkView1).ShouldBeTheSameAs(_sparkView3);
        }
    }
}