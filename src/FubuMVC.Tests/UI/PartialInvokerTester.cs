using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI
{ 
    public abstract class when_the_partial_request_is_authorized_by_object : InteractionContext<PartialInvoker>
    {
		protected PartialInputModel theInput;
        protected IActionBehavior theAction;

    	protected abstract void Invoke();
    	protected abstract void Configure();
		
        protected override void beforeEach()
        {
            theInput = new PartialInputModel();
            theAction = MockFor<IActionBehavior>();

            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(true);
            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(typeof(PartialInputModel))).Return(theAction);

            MockFor<IOutputWriter>()
                .Expect(x => x.Record(theAction.InvokePartial))
                .WhenCalled(r => theAction.InvokePartial())
                .Return(MockFor<IRecordedOutput>());

            Services.Inject<ITypeResolver>(new TypeResolver());

        	Configure();
        	Invoke();
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
            theAction.AssertWasCalled(x => x.InvokePartial());
        }

        [Test]
        public void should_record_on_the_outputwriter()
        {
            MockFor<IOutputWriter>().VerifyAllExpectations();
        }

        [Test]
        public void should_return_recorded_output()
        {
            MockFor<IRecordedOutput>().AssertWasCalled(x => x.GetText());
        }
    }

	[TestFixture]
	public class when_the_partial_request_is_authorized_by_object_with_model_binding : when_the_partial_request_is_authorized_by_object
	{
		protected override void Configure()
		{
			MockFor<ISetterBinder>()
				.Expect(x => x.BindProperties(theInput.GetType(), theInput));
		}

		protected override void Invoke()
		{
			ClassUnderTest.InvokeObject(theInput, true);
		}

		[Test]
		public void should_bind_properties_on_the_input()
		{
			MockFor<ISetterBinder>().VerifyAllExpectations();
		}
	}

	[TestFixture]
	public class when_the_partial_request_is_authorized_by_object_without_model_binding : when_the_partial_request_is_authorized_by_object
	{
		protected override void Configure()
		{
			
		}

		protected override void Invoke()
		{
			ClassUnderTest.InvokeObject(theInput, false);
		}

		[Test]
		public void should_not_bind_properties_on_the_input()
		{
			MockFor<ISetterBinder>().AssertWasNotCalled(x => x.BindProperties(theInput.GetType(), theInput));
		}
	}

	[TestFixture]
	public class when_the_partial_request_is_authorized_by_object_with_model_binding_unspecified : when_the_partial_request_is_authorized_by_object
	{
		protected override void Configure()
		{

		}

		protected override void Invoke()
		{
			ClassUnderTest.InvokeObject(theInput);
		}

		[Test]
		public void should_not_bind_properties_on_the_input()
		{
			MockFor<ISetterBinder>().AssertWasNotCalled(x => x.BindProperties(theInput.GetType(), theInput));
		}
	}

    [TestFixture]
    public class when_the_partial_request_is_authorized : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;
        private IActionBehavior theAction;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();
            theAction = MockFor<IActionBehavior>();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(true);
            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(typeof (PartialInputModel))).Return(theAction);

            MockFor<IOutputWriter>()
                .Expect(x => x.Record(theAction.InvokePartial))
                .WhenCalled(r => theAction.InvokePartial())
                .Return(MockFor<IRecordedOutput>());

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
            theAction.AssertWasCalled(x => x.InvokePartial());
        }

        [Test]
        public void should_record_on_the_outputwriter()
        {
            MockFor<IOutputWriter>().VerifyAllExpectations();
        }

        [Test]
        public void should_return_recorded_output()
        {
            MockFor<IRecordedOutput>().AssertWasCalled(x => x.GetText());
        }
    }


    [TestFixture]
    public class when_the_partial_request_is_not_authorized : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;
        private string theOutput;
        private IActionBehavior theAction;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();
            theAction = MockFor<IActionBehavior>();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(false);
            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(typeof(PartialInputModel))).Return(theAction);

            theOutput = ClassUnderTest.Invoke<PartialInputModel>();
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
            theAction.AssertWasNotCalled(x => x.InvokePartial());
        }

        [Test]
        public void should_not_record_on_the_outputwriter()
        {
            MockFor<IOutputWriter>().AssertWasNotCalled(x => x.Record(theAction.InvokePartial));
        }

        [Test]
        public void should_return_empty_result()
        {
            theOutput.ShouldBeEmpty();
        }
    }


    [TestFixture]
    public class when_the_partial_request_is_not_authorized_by_object : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;
        private string theOutput;
        private IActionBehavior theAction;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();
            theAction = MockFor<IActionBehavior>();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(false);
            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(typeof(PartialInputModel))).Return(theAction);
            Services.Inject<ITypeResolver>(new TypeResolver());

            theOutput = ClassUnderTest.InvokeObject(theInput);
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

        [Test]
        public void should_not_record_on_the_outputwriter()
        {
            MockFor<IOutputWriter>().AssertWasNotCalled(x => x.Record(theAction.InvokePartial));
        }

        [Test]
        public void should_return_empty_result()
        {
            theOutput.ShouldBeEmpty();
        }

        [Test]
        public void should_not_bind_properties_on_the_input()
        {
            MockFor<ISetterBinder>().AssertWasNotCalled(x => x.BindProperties(theInput.GetType(), theInput));
        }
    }


    public class PartialInputModel{}
}