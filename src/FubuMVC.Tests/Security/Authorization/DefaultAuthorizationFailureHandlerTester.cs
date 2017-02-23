using System.Net;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authorization
{
    
    public class DefaultAuthorizationFailureHandlerTester : InteractionContext<DefaultAuthorizationFailureHandler>
    {
        [Fact]
        public void should_set_the_status_code_to_forbidden()
        {
            ClassUnderTest.Handle().AssertWasEndedWithStatusCode(HttpStatusCode.Forbidden);
        }
    }
}