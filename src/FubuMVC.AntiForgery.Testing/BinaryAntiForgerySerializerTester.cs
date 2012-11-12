using System;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.AntiForgery.Testing
{
    [TestFixture]
    public class BinaryAntiForgerySerializerTester : InteractionContext<BinaryAntiForgerySerializer>
    {
        public class TestEncoder : IAntiForgeryEncoder
        {
            public byte[] Decode(string value)
            {
                return Convert.FromBase64String(value);
            }

            public string Encode(byte[] bytes)
            {
                return Convert.ToBase64String(bytes);
            }
        }

        protected override void beforeEach()
        {
            Services.Inject(typeof (IAntiForgeryEncoder), new TestEncoder());
        }


        [Test]
        public void deserialization_is_asp_compatible()
        {
            const string aspSerialized = "CFRoZSBTYWx0CVRoZSBWYWx1ZQBAik+Cb80IDFRoZSBVc2VybmFtZQ==";

            //Base64 Serialized version of:
            var token = new AntiForgeryData
            {
                Salt = "The Salt",
                Username = "The Username",
                Value = "The Value",
                CreationDate = new DateTime(2010, 12, 22),
            };

            AntiForgeryData deserialized = ClassUnderTest.Deserialize(aspSerialized);


            deserialized.CreationDate.ShouldEqual(token.CreationDate);
            deserialized.Salt.ShouldEqual(token.Salt);
            deserialized.Username.ShouldEqual(token.Username);
            deserialized.Value.ShouldEqual(token.Value);
        }

        [Test]
        public void serialization_and_deserialization_match()
        {
            var token = new AntiForgeryData
            {
                CreationDate = new DateTime(2011, 1, 30),
                Salt = "Testing",
                Username = "User",
                Value = "12345"
            };
            string serialized = ClassUnderTest.Serialize(token);
            AntiForgeryData deserialized = ClassUnderTest.Deserialize(serialized);

            deserialized.CreationDate.ShouldEqual(token.CreationDate);
            deserialized.Salt.ShouldEqual(token.Salt);
            deserialized.Username.ShouldEqual(token.Username);
            deserialized.Value.ShouldEqual(token.Value);
        }


        [Test]
        public void serialization_is_asp_compatible()
        {
            const string aspSerialized = "CFRoZSBTYWx0CVRoZSBWYWx1ZQBAik+Cb80IDFRoZSBVc2VybmFtZQ==";

            //Base64 Serialized version of:
            var token = new AntiForgeryData
            {
                Salt = "The Salt",
                Username = "The Username",
                Value = "The Value",
                CreationDate = new DateTime(2010, 12, 22),
            };

            string deserialized = ClassUnderTest.Serialize(token);

            deserialized.ShouldEqual(aspSerialized);
        }
    }
}