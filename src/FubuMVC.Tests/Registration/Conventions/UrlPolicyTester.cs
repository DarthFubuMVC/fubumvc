using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class UrlPolicyTester
    {
        private UrlPolicy _policy;
        private IRouteInputPolicy _inputPolicy;

        [SetUp]
        public void SetUp()
        {
            _inputPolicy = MockRepository.GenerateStub<IRouteInputPolicy>();
            _policy = new UrlPolicy(c => true);
        }

        [Test]
        public void should_use_namespace_classname_and_method_name_for_url()
        {
            Output().ShouldEqual("fubumvc/tests/registration/conventions/urlpolicycontroller/somemethod");
        }

        [Test]
        public void should_respect_IgnoreClassSuffix()
        {
            _policy.IgnoreClassSuffix("controller");
            Output().ShouldEqual("fubumvc/tests/registration/conventions/urlpolicy/somemethod");
        }

        [Test]
        public void should_respect_IgnoreClassName()
        {
            _policy.IgnoreClassName(typeof(UrlPolicyController));
            Output().ShouldEqual("fubumvc/tests/registration/conventions/somemethod");
        }

        [Test]
        public void should_respect_IgnoreMethods()
        {
            _policy.IgnoreMethods("somemethod");
            Output().ShouldEqual("fubumvc/tests/registration/conventions/urlpolicycontroller");
        }

        [Test]
        public void should_respect_IgnoreNamespace()
        {
            _policy.IgnoreNamespace("FubuMVC.Tests.Registration");
            Output().ShouldEqual("conventions/urlpolicycontroller/somemethod");
        }

        [Test]
        public void should_respect_RegisterMethodNameStrategy()
        {
            _policy.RegisterMethodNameStrategy(m=>true, m=>"foo");
            Output().ShouldEqual("foo");
        }

        private string Output()
        {
            return _policy.Build(buildCall(), _inputPolicy).ToRoute().Url;
        }

        private ActionCall buildCall()
        {
            return buildCall<UrlPolicyController>(c=>c.SomeMethod(null));
        }

        private ActionCall buildCall<T>(Expression<Action<T>> expression)
        {
            var method = ReflectionHelper.GetMethod(expression);
            return new ActionCall(typeof(T), method);
        }

        public class UrlPolicyController
        {
            public void SomeMethod(object model)
            {
            }
        }
    }
}