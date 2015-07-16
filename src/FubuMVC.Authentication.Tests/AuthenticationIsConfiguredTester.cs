using System.Linq;
using Bottles.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Authentication.Tests
{
    [TestFixture]
    public class AuthenticationIsConfiguredTester : InteractionContext<AuthenticationIsConfigured>
    {
        [Test]
        public void everything_is_wonderful_if_there_is_at_least_one_strategy()
        {
            Services.CreateMockArrayFor<IAuthenticationStrategy>(3);

            var packageLog = MockFor<IPackageLog>();
            ClassUnderTest.Activate(packageLog);

            packageLog.AssertWasNotCalled(x => x.MarkFailure("text"), x => x.IgnoreArguments());
        }

        [Test]
        public void logs_an_error_if_there_are_no_IAuthenticationStrategys_registered()
        {
            Services.CreateMockArrayFor<IAuthenticationStrategy>(0);

            var packageLog = MockFor<IPackageLog>();

            var activator = new AuthenticationIsConfigured(Enumerable.Empty<IAuthenticationStrategy>());

            activator.Activate(packageLog);

            packageLog.AssertWasCalled(x => x.MarkFailure("There are no IAuthenticationStrategy services registered.  Either register an IAuthenticationStrategy or remove FubuMVC.Authentication from your application"));
        }
    }
}