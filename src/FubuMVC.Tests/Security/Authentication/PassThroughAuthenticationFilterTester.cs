using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication
{
	[TestFixture]
	public class PassThroughAuthenticationFilterTester : InteractionContext<PassThroughAuthenticationFilter>
	{
		private FubuContinuation theContinuation;

		protected override void beforeEach()
		{
			MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(false);
			theContinuation = ClassUnderTest.Filter();
		}

		[Test]
		public void continues_to_the_next_behavior()
		{
			theContinuation.AssertWasContinuedToNextBehavior();
		}

		[Test]
		public void applies_authentication()
		{
			MockFor<IAuthenticationService>().AssertWasCalled(x => x.TryToApply());
		}
	}
}