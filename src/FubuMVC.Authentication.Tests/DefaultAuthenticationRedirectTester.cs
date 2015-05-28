using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;


namespace FubuMVC.Authentication.Tests
{
	[TestFixture]
	public class DefaultAuthenticationRedirectTester : InteractionContext<DefaultAuthenticationRedirect>
	{
		[Test]
		public void always_matches()
		{
			ClassUnderTest.Applies().ShouldBeTrue();
		}

	    [Test]
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