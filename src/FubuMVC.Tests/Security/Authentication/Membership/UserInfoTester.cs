﻿using System.Linq;
using FubuMVC.Core.Security.Authentication.Membership;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication.Membership
{
    
    public class UserInfoTester
    {
        [Fact]
        public void get_and_set()
        {
            var foo = new Foo {Name = "Scooby"};
            var bar = new Bar {Level = 33};

            var user = new UserInfo();
            user.Set(foo);
            user.Set(bar);

            user.Get<Foo>().ShouldBeTheSameAs(foo);
            user.Get<Bar>().ShouldBeTheSameAs(bar);
        }

        [Fact]
        public void add_roles()
        {
            var user = new UserInfo();
            user.Roles.Any().ShouldBeFalse();

            user.Roles = new string[]{"A", "B"};

            user.AddRoles("C");

            user.Roles.ShouldHaveTheSameElementsAs("A", "B", "C");
        }
    }

    public class Foo
    {
        public string Name { get; set; }
    }

    public class Bar
    {
        public int Level { get; set; }
    }
}