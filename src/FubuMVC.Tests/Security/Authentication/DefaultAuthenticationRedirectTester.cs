using FubuMVC.Core.Http;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication
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