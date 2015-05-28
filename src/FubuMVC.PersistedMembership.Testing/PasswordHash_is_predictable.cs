using System.Diagnostics;
using FubuMVC.Authentication;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.PersistedMembership.Testing
{
    [TestFixture]
    public class PasswordHash_is_predictable
    {
        [Test]
        public void see_it_in_action()
        {
            var hash = new PasswordHash();
            var password = "something";

            for (int i = 0; i < 50; i++)
            {
                Debug.WriteLine(hash.CreateHash(password));
            }

                hash.CreateHash(password).ShouldEqual(hash.CreateHash(password));
            hash.CreateHash(password).ShouldEqual(hash.CreateHash(password));
            hash.CreateHash(password).ShouldEqual(hash.CreateHash(password));
            hash.CreateHash(password).ShouldEqual(hash.CreateHash(password));

            hash.CreateHash(password).ShouldNotEqual(password);
        }
    }
}