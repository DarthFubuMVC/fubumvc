using System;
using System.Web;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Is = Rhino.Mocks.Constraints.Is;

namespace FubuMVC.Tests.Runtime
{
    public class CaptureArg : AbstractConstraint
    {
        private object _obj;

        public override string Message { get { return "Ok"; } }

        public override bool Eval(object obj)
        {
            _obj = obj;
            return true;
        }

        public T Get<T>()
        {
            _obj.ShouldNotBeNull();
            return _obj.ShouldBeOfType<T>();
        }
    }

    [TestFixture]
    public class FubuRouteHandlerTester : InteractionContext<FubuRouteHandler>
    {
        protected override void beforeEach()
        {
            //NOTE: I'm told that the following 4 lines are the result of making things 'easier to mock'
            var context =
                new RequestContext(
                    new HttpContextWrapper(new HttpContext(new HttpRequest("foo.txt", "http://test", ""),
                                                           new HttpResponse(Console.Out))),
                    new RouteData());

            var chain = new BehaviorChain();
            Services.Inject(chain);

            captured = new CaptureArg();

            MockFor<IBehaviorFactory>()
                .Expect(x => x.BuildBehavior(null, behaviorId))
                .Constraints(captured, Is.Equal(behaviorId))
                .Return(MockFor<IActionBehavior>());

            handler = ClassUnderTest.GetHttpHandler(context);
        }


        private Guid behaviorId;
        private IHttpHandler handler;
        private CaptureArg captured;

        [Test]
        public void should_call_into_behavior_factory_to_build_the_handler()
        {
            VerifyCallsFor<IBehaviorFactory>();
        }

        [Test]
        public void should_pass_an_aggregate_dictionary_into_the_service_arguments()
        {
            var args = captured.Get<ServiceArguments>();
            args.Get<AggregateDictionary>().ShouldNotBeNull();
        }

        [Test]
        public void the_handler_should_call_the_behavior_build_by_the_behavior_factory()
        {
            handler.ProcessRequest(null);
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }
}