using System.Net;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class DefaultAuthorizationFailureHandlerTester : InteractionContext<DefaultAuthorizationFailureHandler>
    {
        [Test]
        public void should_set_the_status_code_to_forbidden()
        {
            ClassUnderTest.Handle();
            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.Forbidden));
        }
    }
}