using FubuMVC.Core.Security.Authentication.Tickets;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication.Tickets
{
    [TestFixture]
    public class EncryptionTester
    {
        private Encryptor theEncryptor;

        [SetUp]
        public void SetUp()
        {
            theEncryptor = new Encryptor(new EncryptionSettings());
        }

        [Test]
        public void is_predictable()
        {
            var plain = "the rain in spain";
            var encrypted1 = theEncryptor.Encrypt(plain);
            var encrypted2 = theEncryptor.Encrypt(plain);

            encrypted1.ShouldNotBe(plain);

            encrypted1.ShouldBe(encrypted2);
        }

        [Test]
        public void can_round_trip()
        {
            var plain = "the rain in spain";
            var encrypted1 = theEncryptor.Encrypt(plain);

            theEncryptor.Decrypt(encrypted1).ShouldBe(plain);
        }
    }
}