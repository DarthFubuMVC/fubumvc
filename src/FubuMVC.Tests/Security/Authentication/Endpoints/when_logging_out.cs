using System.Net;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication.Endpoints
{
	
	public class when_logging_out : InteractionContext<LogoutController>
	{
		private FubuContinuation theContinuation;
	    private FubuContinuation expectedContinuation;

	    protected override void beforeEach()
		{
		    expectedContinuation = FubuContinuation.EndWithStatusCode(HttpStatusCode.Accepted);
            MockFor<ILogoutSuccessHandler>().Stub(x => x.LoggedOut()).Return(expectedContinuation);

			theContinuation = ClassUnderTest.Logout(null);
		}

		[Fact]
		public void should_clear_the_authentication()
		{
			MockFor<IAuthenticationSession>().AssertWasCalled(x => x.ClearAuthentication());
		}

		[Fact]
		public void should_return_the_continuation_from_the_registered_logout_handler()
		{
		    theContinuation.ShouldBeTheSameAs(expectedContinuation);
		}
	}
}