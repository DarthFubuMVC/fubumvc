using FubuMVC.Core.Security.Authentication.Tickets;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication.Tickets
{
    
    public class EncryptionTester
    {
        private Encryptor theEncryptor = new Encryptor(new EncryptionSettings());

        [Fact]
        public void is_predictable()
        {
            var plain = "the rain in spain";
            var encrypted1 = theEncryptor.Encrypt(plain);
            var encrypted2 = theEncryptor.Encrypt(plain);

            encrypted1.ShouldNotBe(plain);

            encrypted1.ShouldBe(encrypted2);
        }

        [Fact]
        public void can_round_trip()
        {
            var plain = "the rain in spain";
            var encrypted1 = theEncryptor.Encrypt(plain);

            theEncryptor.Decrypt(encrypted1).ShouldBe(plain);
        }
    }
}