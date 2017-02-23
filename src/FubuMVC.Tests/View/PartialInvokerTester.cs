using System.Linq;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.Urls;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI
{ 
    public abstract class when_the_partial_request_is_authorized_by_object : InteractionContext<PartialInvoker>
    {
		protected PartialInputModel theInput;
        protected IActionBehavior theAction;
        private BehaviorChain theChain;

        protected abstract void Invoke();
    	protected abstract void Configure();
		
        protected override void beforeEach()
        {
            theInput = new PartialInputModel();
            theChain = new BehaviorChain();

            MockFor<IChainResolver>().Stub(x => x.FindUniqueByType(theInput.GetType(), Categories.VIEW)).Return(theChain);

            theAction = MockFor<IActionBehavior>();

            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(true);
            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(theChain)).Return(theAction);
            MockFor<IRecordedOutput>().Stub(x => x.Headers()).Return(Enumerable.Empty<Header>());

            MockFor<IOutputWriter>()
                .Expect(x => x.Record(theAction.InvokePartial))
                .WhenCalled(r => theAction.InvokePartial())
                .Return(MockFor<IRecordedOutput>());

        	Configure();
        	Invoke();
        }

        [Fact]
        public void should_store_the_model()
        {
            MockFor<IFubuRequest>().AssertWasCalled(x => x.Set(typeof(PartialInputModel), theInput));
        }

        [Fact]
        public void should_have_tested_the_authorization()
        {
            MockFor<IAuthorizationPreviewService>().VerifyAllExpectations();
        }

        [Fact]
        public void should_invoke_the_partial_behavior()
        {
            theAction.AssertWasCalled(x => x.InvokePartial());
        }

        [Fact]
        public void should_record_on_the_outputwriter()
        {
            MockFor<IOutputWriter>().VerifyAllExpectations();
        }

        [Fact]
        public void should_return_recorded_output()
        {
            MockFor<IRecordedOutput>().AssertWasCalled(x => x.GetText());
        }
    }

	
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

		[Fact]
		public void should_bind_properties_on_the_input()
		{
			MockFor<ISetterBinder>().VerifyAllExpectations();
		}
	}

	
	public class when_the_partial_request_is_authorized_by_object_without_model_binding : when_the_partial_request_is_authorized_by_object
	{
		protected override void Configure()
		{
			
		}

		protected override void Invoke()
		{
			ClassUnderTest.InvokeObject(theInput, false);
		}

		[Fact]
		public void should_not_bind_properties_on_the_input()
		{
			MockFor<ISetterBinder>().AssertWasNotCalled(x => x.BindProperties(theInput.GetType(), theInput));
		}
	}

	
	public class when_the_partial_request_is_authorized_by_object_with_model_binding_unspecified : when_the_partial_request_is_authorized_by_object
	{
		protected override void Configure()
		{

		}

		protected override void Invoke()
		{
			ClassUnderTest.InvokeObject(theInput);
		}

		[Fact]
		public void should_not_bind_properties_on_the_input()
		{
			MockFor<ISetterBinder>().AssertWasNotCalled(x => x.BindProperties(theInput.GetType(), theInput));
		}
	}

    
    public class when_the_partial_request_is_authorized : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;
        private IActionBehavior theAction;
        private BehaviorChain theChain;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();
            theAction = MockFor<IActionBehavior>();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput, null)).Return(true);


            theChain = new BehaviorChain();
            MockFor<IChainResolver>().Stub(x => x.FindUniqueByType(typeof (PartialInputModel), Categories.VIEW)).Return(theChain);

            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(theChain)).Return(theAction);
            MockFor<IRecordedOutput>().Stub(x => x.Headers()).Return(Enumerable.Empty<Header>());

            MockFor<IOutputWriter>()
                .Expect(x => x.Record(theAction.InvokePartial))
                .WhenCalled(r => theAction.InvokePartial())
                .Return(MockFor<IRecordedOutput>());

            ClassUnderTest.Invoke<PartialInputModel>();
        }

        [Fact]
        public void should_have_tested_the_authorization()
        {
            MockFor<IAuthorizationPreviewService>().VerifyAllExpectations();
        }

        [Fact]
        public void should_invoke_the_partial_behavior()
        {
            theAction.AssertWasCalled(x => x.InvokePartial());
        }

        [Fact]
        public void should_record_on_the_outputwriter()
        {
            MockFor<IOutputWriter>().VerifyAllExpectations();
        }

        [Fact]
        public void should_return_recorded_output()
        {
            MockFor<IRecordedOutput>().AssertWasCalled(x => x.GetText());
        }
    }


    
    public class when_the_partial_request_is_not_authorized : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;
        private string theOutput;
        private IActionBehavior theAction;
        private BehaviorChain theChain;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();
            theAction = MockFor<IActionBehavior>();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput, null)).Return(false);

            theChain = new BehaviorChain();
            MockFor<IChainResolver>().Stub(x => x.FindUniqueByType(typeof (PartialInputModel), Categories.VIEW)).Return(theChain);
            MockFor<IRecordedOutput>().Stub(x => x.Headers()).Return(Enumerable.Empty<Header>());
            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(theChain)).Return(theAction);

            theOutput = ClassUnderTest.Invoke<PartialInputModel>();
        }

        [Fact]
        public void should_have_tested_the_authorization()
        {
            MockFor<IAuthorizationPreviewService>().VerifyAllExpectations();
        }

        [Fact]
        public void should_not_invoke_the_partial_behavior()
        {
            MockFor<IPartialFactory>().AssertWasNotCalled(x => x.BuildPartial(theChain));
            theAction.AssertWasNotCalled(x => x.InvokePartial());
        }

        [Fact]
        public void should_not_record_on_the_outputwriter()
        {
            MockFor<IOutputWriter>().AssertWasNotCalled(x => x.Record(theAction.InvokePartial));
        }

        [Fact]
        public void should_return_empty_result()
        {
            theOutput.ShouldBeEmpty();
        }
    }


    
    public class when_the_partial_request_is_not_authorized_by_object : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;
        private string theOutput;
        private IActionBehavior theAction;
        private BehaviorChain theChain;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();
            theAction = MockFor<IActionBehavior>();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput)).Return(false);

            theChain = new BehaviorChain();
            MockFor<IRecordedOutput>().Stub(x => x.Headers()).Return(Enumerable.Empty<Header>());

            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(theChain)).Return(theAction);

            theOutput = ClassUnderTest.InvokeObject(theInput);
        }

        [Fact]
        public void should_have_tested_the_authorization()
        {
            MockFor<IAuthorizationPreviewService>().VerifyAllExpectations();
        }

        [Fact]
        public void should_not_invoke_the_partial_behavior()
        {
            MockFor<IPartialFactory>().AssertWasNotCalled(x => x.BuildPartial(theChain));
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.InvokePartial());
        }

        [Fact]
        public void should_not_record_on_the_outputwriter()
        {
            MockFor<IOutputWriter>().AssertWasNotCalled(x => x.Record(theAction.InvokePartial));
        }

        [Fact]
        public void should_return_empty_result()
        {
            theOutput.ShouldBeEmpty();
        }

        [Fact]
        public void should_not_bind_properties_on_the_input()
        {
            MockFor<ISetterBinder>().AssertWasNotCalled(x => x.BindProperties(theInput.GetType(), theInput));
        }
    }

    
    public class when_the_POST_partial_request_is_authorized : InteractionContext<PartialInvoker>
    {
        private PartialInputModel theInput;
        private IActionBehavior theAction;
        private BehaviorChain theChain;

        protected override void beforeEach()
        {
            theInput = new PartialInputModel();
            theAction = MockFor<IActionBehavior>();

            MockFor<IFubuRequest>().Stub(x => x.Get<PartialInputModel>()).Return(theInput);
            MockFor<IAuthorizationPreviewService>().Expect(x => x.IsAuthorized(theInput, "POST")).Return(true);


            theChain = new BehaviorChain();
            MockFor<IChainResolver>().Stub(x => x.FindUniqueByType(typeof(PartialInputModel), category: "POST")).Return(theChain);

            MockFor<IPartialFactory>().Stub(x => x.BuildPartial(theChain)).Return(theAction);
            MockFor<IRecordedOutput>().Stub(x => x.Headers()).Return(Enumerable.Empty<Header>());

            MockFor<IOutputWriter>()
                .Expect(x => x.Record(theAction.InvokePartial))
                .WhenCalled(r => theAction.InvokePartial())
                .Return(MockFor<IRecordedOutput>());

            ClassUnderTest.Invoke<PartialInputModel>("POST");
        }

        [Fact]
        public void should_have_tested_the_authorization()
        {
            MockFor<IAuthorizationPreviewService>().VerifyAllExpectations();
        }

        [Fact]
        public void should_invoke_the_partial_behavior()
        {
            theAction.AssertWasCalled(x => x.InvokePartial());
        }

        [Fact]
        public void should_record_on_the_outputwriter()
        {
            MockFor<IOutputWriter>().VerifyAllExpectations();
        }

        [Fact]
        public void should_return_recorded_output()
        {
            MockFor<IRecordedOutput>().AssertWasCalled(x => x.GetText());
        }
    }


    public class PartialInputModel{}
}