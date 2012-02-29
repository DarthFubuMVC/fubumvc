using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class DefaultRouteInputTypeBasedUrlPolicyTester
    {
        private MethodInfo _method;
        private DefaultRouteInputTypeBasedUrlPolicy _policy;
        private RecordingConfigurationObserver _log;

        [SetUp]
        public void SetUp()
        {
            _method = ReflectionHelper.GetMethod<TestController>(c => c.SomeAction(null));
            _policy = new DefaultRouteInputTypeBasedUrlPolicy(typeof(TestInputModel));
            _log = new RecordingConfigurationObserver();
        }

        [Test]
        public void should_match_the_action_call_input_type()
        {
            var call = new ActionCall(typeof(TestController), _method);
            _policy.Matches(call, _log).ShouldBeTrue();
        }


        [Test]
        public void should_log_when_default_route_found()
        {
            var call = new ActionCall(typeof(TestController), _method);
            _policy.Matches(call, _log);

            _log.GetLog(call).ShouldNotBeEmpty();
        }

        [Test]
        public void should_not_match_the_action_call_if_the_input_type_is_different()
        {
            var otherMethod = ReflectionHelper.GetMethod<TestController>(c => c.SomeAction(0));
            var call = new ActionCall(typeof(TestController), otherMethod);
            _policy.Matches(call, _log).ShouldBeFalse();
        }

        [Test]
        public void should_throw_if_more_than_one_call_has_the_same_input_type()
        {
            var firstMethod = ReflectionHelper.GetMethod<TestController>(c => c.SomeAction(null));
            var otherMethod = ReflectionHelper.GetMethod<TestController>(c => c.AnotherAction(null));
            var policy = new DefaultRouteInputTypeBasedUrlPolicy(typeof(TestInputModel));

            var firstCall = new ActionCall(typeof (TestController), firstMethod);
            var otherCall = new ActionCall(typeof(TestController), otherMethod);

            policy.Matches(firstCall, _log);

            typeof(FubuException).ShouldBeThrownBy(() => policy.Matches(otherCall, _log));
        }

        [Test]
        public void should_build_a_route_definition_from_the_action_call()
        {
            var call = new ActionCall(typeof(TestController), _method);
            _policy.Build(call).ShouldNotBeNull();
        }
    }
}