using System.Net;
using FubuMVC.Core.Security.Authorization;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authorization
{
    [TestFixture]
    public class DefaultAuthorizationFailureHandlerTester : InteractionContext<DefaultAuthorizationFailureHandler>
    {
        [Test]
        public void should_set_the_status_code_to_forbidden()
        {
            ClassUnderTest.Handle().AssertWasEndedWithStatusCode(HttpStatusCode.Forbidden);
        }
    }
}