using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication
{
	
	public class pass_through_authentication_for_a_partial : InteractionContext<PassThroughAuthenticationFilter>
	{
		private FubuContinuation theContinuation;

		protected override void beforeEach()
		{
			MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(true);
			theContinuation = ClassUnderTest.Filter();
		}

		[Fact]
		public void continues_to_the_next_behavior()
		{
			theContinuation.AssertWasContinuedToNextBehavior();
		}

		[Fact]
		public void does_not_apply_to_authentication()
		{
			MockFor<IAuthenticationService>().AssertWasNotCalled(x => x.TryToApply());
		}
	}
}