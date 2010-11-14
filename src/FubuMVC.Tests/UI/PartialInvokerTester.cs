using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class when_the_partial_request_is_authorized_by_object : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();

            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(true);

            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(typeof(PartialInputModel))).Return(
                MockFor<IActionBehavior>());

            Services.Inject<ITypeResolver>(new TypeResolver());

            ClassUnderTest.InvokeObject(theInput);
        }

        [Test]
        public void should_store_the_model()
        {
            MockFor<IFubuRequest>().AssertWasCalled(x => x.Set(typeof(PartialInputModel), theInput));
        }

        [Test]
        public void should_have_tested_the_authorization()
        {
            MockFor<IAuthorizationPreviewService>().VerifyAllExpectations();
        }

        [Test]
        public void should_invoke_the_partial_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.InvokePartial());
        }
    }



    [TestFixture]
    public class when_the_partial_request_is_authorized : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(true);

            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(typeof (PartialInputModel))).Return(
                MockFor<IActionBehavior>());

            ClassUnderTest.Invoke<PartialInputModel>();
        }

        [Test]
        public void should_have_tested_the_authorization()
        {
            MockFor<IAuthorizationPreviewService>().VerifyAllExpectations();
        }

        [Test]
        public void should_invoke_the_partial_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class when_the_partial_request_is_not_authorized : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(false);

            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(typeof(PartialInputModel))).Return(
            MockFor<IActionBehavior>());

            ClassUnderTest.Invoke<PartialInputModel>();
        }

        [Test]
        public void should_have_tested_the_authorization()
        {
            MockFor<IAuthorizationPreviewService>().VerifyAllExpectations();
        }

        [Test]
        public void should_not_invoke_the_partial_behavior()
        {
            MockFor<IPartialFactory>().AssertWasNotCalled(x => x.BuildPartial(typeof(PartialInputModel)));
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.InvokePartial());
        }
    }


    [TestFixture]
    public class when_the_partial_request_is_not_authorized_by_object : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(false);

            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(typeof(PartialInputModel))).Return(
            MockFor<IActionBehavior>());

            ClassUnderTest.InvokeObject(theInput);
        }

        [Test]
        public void should_have_tested_the_authorization()
        {
            MockFor<IAuthorizationPreviewService>().VerifyAllExpectations();
        }

        [Test]
        public void should_not_invoke_the_partial_behavior()
        {
            MockFor<IPartialFactory>().AssertWasNotCalled(x => x.BuildPartial(typeof(PartialInputModel)));
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.InvokePartial());
        }
    }


    public class PartialInputModel{}
}