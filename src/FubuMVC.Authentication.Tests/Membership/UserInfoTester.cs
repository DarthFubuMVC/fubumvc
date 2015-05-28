using FubuMVC.Authentication.Membership;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Authentication.Tests.Membership
{
    [TestFixture]
    public class UserInfoTester
    {
        [Test]
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

        [Test]
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