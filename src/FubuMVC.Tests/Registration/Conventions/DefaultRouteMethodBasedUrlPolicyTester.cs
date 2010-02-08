using System.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Util;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class DefaultRouteMethodBasedUrlPolicyTester
    {
        private MethodInfo _method;
        private DefaultRouteMethodBasedUrlPolicy _policy;
        private NulloConfigurationObserver _log;

        [SetUp]
        public void SetUp()
        {
            _method = ReflectionHelper.GetMethod<TestController>(c => c.SomeAction(null));
            _policy = new DefaultRouteMethodBasedUrlPolicy(_method);
            _log = new NulloConfigurationObserver();
        }

        [Test]
        public void should_match_the_action_call_method()
        {
            var method = ReflectionHelper.GetMethod<TestController>(c => c.SomeAction(null));
            var call = new ActionCall(typeof(TestController), method);
            _policy.Matches(call, _log).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_another_method()
        {
            var anotherMethod = ReflectionHelper.GetMethod<TestController>(c => c.AnotherAction(null));
            var call = new ActionCall(typeof(TestController), anotherMethod);
            _policy.Matches(call, _log).ShouldBeFalse();
        }

        [Test]
        public void should_build_a_route_definition_from_the_action_call()
        {
            var call = new ActionCall(typeof(TestController), _method);
            _policy.Build(call).ShouldNotBeNull();
        }
    }
}