using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class FubuHttpHandlerTester : InteractionContext<FubuRouteHandler.FubuHttpHandler>
    {
        [Test]
        public void execute_should_delegate_to_the_behavior()
        {
            ClassUnderTest.ProcessRequest(null);
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }
}