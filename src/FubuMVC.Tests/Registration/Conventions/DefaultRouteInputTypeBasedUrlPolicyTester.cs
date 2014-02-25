using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class DefaultRouteInputTypeBasedUrlPolicyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _method = ReflectionHelper.GetMethod<TestController>(c => c.SomeAction(null));
            _policy = new DefaultRouteInputTypeBasedUrlPolicy(typeof (TestInputModel));
        }

        #endregion

        private MethodInfo _method;
        private DefaultRouteInputTypeBasedUrlPolicy _policy;

        [Test]
        public void should_build_a_route_definition_from_the_action_call()
        {
            var call = new ActionCall(typeof (TestController), _method);
            _policy.Build(call).ShouldNotBeNull();
        }


        [Test]
        public void should_match_the_action_call_input_type()
        {
            var call = new ActionCall(typeof (TestController), _method);
            _policy.Matches(call).ShouldBeTrue();
        }


        [Test]
        public void should_throw_if_more_than_one_call_has_the_same_input_type()
        {
            var firstMethod = ReflectionHelper.GetMethod<TestController>(c => c.SomeAction(null));
            var otherMethod = ReflectionHelper.GetMethod<TestController>(c => c.AnotherAction(null));
            var policy = new DefaultRouteInputTypeBasedUrlPolicy(typeof (TestInputModel));

            var firstCall = new ActionCall(typeof (TestController), firstMethod);
            var otherCall = new ActionCall(typeof (TestController), otherMethod);

            policy.Matches(firstCall);

            typeof (FubuException).ShouldBeThrownBy(() => policy.Matches(otherCall));
        }
    }
}