using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.Rendering
{
    [TestFixture]
    public class ViewModifierServiceTester : InteractionContext<ViewModifierService<IFubuRazorView>>
    {
        private FubuRazorView _razorView1;
        private FubuRazorView _razorView2;
        private FubuRazorView _razorView3;
        private IViewModifier<IFubuRazorView> _modification1;
        private IViewModifier<IFubuRazorView> _modification2;
        private IViewModifier<IFubuRazorView> _modification3;

        protected override void beforeEach()
        {
            var modifications = Services.CreateMockArrayFor<IViewModifier<IFubuRazorView>>(3);
            _razorView1 = MockRepository.GenerateMock<FubuRazorView>();
            _razorView2 = MockRepository.GenerateMock<FubuRazorView>();
            _razorView3 = MockRepository.GenerateMock<FubuRazorView>();
            _modification1 = modifications[0];
            _modification2 = modifications[1];
            _modification3 = modifications[2];

            _modification1.Expect(x => x.Applies(_razorView1)).Return(true);
            _modification2.Expect(x => x.Applies(_razorView2)).Return(false);
            _modification3.Expect(x => x.Applies(_razorView2)).Return(true);

            _modification1.Expect(x => x.Modify(_razorView1)).Return(_razorView2);
            _modification2.Expect(x => x.Modify(Arg<IFubuRazorView>.Is.Anything)).Repeat.Never();
            _modification3.Expect(x => x.Modify(_razorView2)).Return(_razorView3);
        }

        [Test]
        public void only_applicable_modifications_are_used()
        {
            ClassUnderTest.Modify(_razorView1);
            _modification1.VerifyAllExpectations();
            _modification2.VerifyAllExpectations();
            _modification3.VerifyAllExpectations();
        }

        [Test]
        public void the_resulting_view_comes_from_the_modifiers()
        {
            ClassUnderTest.Modify(_razorView1).ShouldBeTheSameAs(_razorView3);
        }
    }
}