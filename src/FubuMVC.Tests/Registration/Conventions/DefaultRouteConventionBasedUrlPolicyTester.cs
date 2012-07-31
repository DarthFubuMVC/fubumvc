﻿using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class DefaultRouteConventionBasedUrlPolicyTester
    {
        private MethodInfo _method;
        private DefaultRouteConventionBasedUrlPolicy _policy;
        private NulloConfigurationObserver _log;

        [SetUp]
        public void SetUp()
        {
            _method = ReflectionHelper.GetMethod<TestController>(c => c.SomeAction(null));
            _policy = new DefaultRouteConventionBasedUrlPolicy();
            _log = new NulloConfigurationObserver();
        }

        [Test]
        public void should_build_a_route_definition_from_the_action_call()
        {
            var call = new ActionCall(typeof(HomeEndpoint), _method);
            _policy.Build(call).ShouldNotBeNull();
        }

        [Test]
        public void should_match_the_home_endpoint_index_method()
        {
            var index = ReflectionHelper.GetMethod<HomeEndpoint>(c => c.Index());
            var call = new ActionCall(typeof(HomeEndpoint), index);
            _policy.Matches(call, _log).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_another_method()
        {
            var anotherMethod = ReflectionHelper.GetMethod<HomeEndpoint>(c => c.Action());
            var call = new ActionCall(typeof(HomeEndpoint), anotherMethod);
            _policy.Matches(call, _log).ShouldBeFalse();
        }

        [Test]
        public void should_not_match_another_controller()
        {
            var anotherMethod = ReflectionHelper.GetMethod<AnotherEndpoint>(c => c.Index());
            var call = new ActionCall(typeof(HomeEndpoint), anotherMethod);
            _policy.Matches(call, _log).ShouldBeFalse();
        } 

        private class HomeEndpoint
        {
            public void Index(){}
            public void Action(){}
        }

        private class AnotherEndpoint
        {
            public void Index() { }
        }
    }
}