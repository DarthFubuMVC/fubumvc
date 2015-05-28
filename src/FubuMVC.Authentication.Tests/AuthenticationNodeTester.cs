using System;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.Authentication.Tests
{
    [TestFixture]
    public class AuthenticationNodeTester
    {
        [Test]
        public void whines_if_you_try_to_use_something_besides_IAuthenticatinoStrategy()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new AuthenticationNode(GetType());
            });
        }

        [Test]
        public void build_the_object_def_successfully()
        {
            var def = new AuthenticationNode(typeof (BasicAuthentication))
                .As<IContainerModel>().ToObjectDef();

            def.Type.ShouldEqual(typeof (BasicAuthentication));

        }
    }
}