using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class RenderStatusCodeBehaviorTester : InteractionContext<RenderStatusCodeBehavior>
    {
        [Test]
        public void invoke_writes_the_status_code_to_the_output()
        {
            MockFor<IFubuRequest>().Stub(x => x.Get(typeof (HttpStatusCode))).Return(HttpStatusCode.Unauthorized);

            ClassUnderTest.Invoke();

            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.Unauthorized));
        }
    }
}