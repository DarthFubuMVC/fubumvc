using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class AsyncBehaviorInvokerTester
    {
        private IServiceFactory theFactory;
        private BehaviorChain theChain;
        private BehaviorInvoker theInvoker;
        private IActionBehavior theBehavior;
        private ServiceArguments theArguments;
        private Dictionary<string, object> theRouteData;

        [SetUp]
        public void SetUp()
        {
            theFactory = MockRepository.GenerateMock<IServiceFactory>();
            theChain = new BehaviorChain()
            {
                Route = new RouteDefinition("something")
            };

            theRouteData = new Dictionary<string, object>();

            theArguments = new ServiceArguments();
            theBehavior = MockRepository.GenerateMock<IActionBehavior>();

            theFactory.Stub(x => x.BuildBehavior(theArguments, theChain.UniqueId))
                .Return(theBehavior);

            theInvoker = new AsyncBehaviorInvoker(theFactory, theChain);
        }

        [Test]
        public void invoke_calls_the_behavior_invoke()
        {
            var testTask = new Task(() => theInvoker.Invoke(theArguments, theRouteData));
            testTask.RunSynchronously();
            theBehavior.AssertWasCalled(x => x.Invoke());
        }
    }
}
   