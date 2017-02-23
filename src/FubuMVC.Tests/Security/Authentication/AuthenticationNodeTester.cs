using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication
{
    
    public class AuthenticationNodeTester
    {
        [Fact]
        public void whines_if_you_try_to_use_something_besides_IAuthenticatinoStrategy()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new AuthenticationNode(GetType());
            });
        }

        [Fact]
        public void build_the_object_def_successfully()
        {
            new AuthenticationNode(typeof (BasicAuthentication))
                .As<IContainerModel>().ToInstance()
                .ReturnedType.ShouldBe(typeof(BasicAuthentication));

        }
    }
}