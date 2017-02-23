using FubuMVC.Core.Http;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication
{
	
	public class DefaultAuthenticationRedirectTester : InteractionContext<DefaultAuthenticationRedirect>
	{
		[Fact]
		public void always_matches()
		{
			ClassUnderTest.Applies().ShouldBeTrue();
		}

	    [Fact]
	    public void redirects_to_the_login_page()
	    {
	        var relativeUrl = "/something";

	        MockFor<IHttpRequest>().Stub(x => x.RelativeUrl())
	                                      .Return(relativeUrl);

            ClassUnderTest.Redirect()
                .AssertWasRedirectedTo(new LoginRequest
                                       {
                                           Url = relativeUrl
                                       }, "GET");
	    }
	}
}