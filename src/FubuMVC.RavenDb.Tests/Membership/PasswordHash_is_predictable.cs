using System.Diagnostics;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.Membership
{
    public class PasswordHash_is_predictable
    {
        [Fact]
        public void see_it_in_action()
        {
            var hash = new PasswordHash();
            var password = "something";

            for (var i = 0; i < 50; i++)
            {
                Debug.WriteLine(hash.CreateHash(password));
            }

            hash.CreateHash(password).ShouldBe(hash.CreateHash(password));
            hash.CreateHash(password).ShouldBe(hash.CreateHash(password));
            hash.CreateHash(password).ShouldBe(hash.CreateHash(password));
            hash.CreateHash(password).ShouldBe(hash.CreateHash(password));
            hash.CreateHash(password).ShouldNotBe(password);
        }
    }
}
