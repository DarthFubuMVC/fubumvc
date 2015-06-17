using System.IO;
using FubuTestingSupport;
using FubuTransportation.Runtime.Serializers;
using NUnit.Framework;

namespace FubuTransportation.Testing.Runtime.Serializers
{
    [TestFixture]
    public class BinarySerializerTester
    {
        [Test]
        public void can_round_trip()
        {
            var address1 = new Address
            {
                City = "Austin",
                State = "Texas"
            };

            var stream = new MemoryStream();
            var serializer = new BinarySerializer();
            serializer.Serialize(address1, stream);

            stream.Position = 0;

            var address2 = serializer.Deserialize(stream).ShouldBeOfType<Address>();
            address1.ShouldEqual(address2);
        }
    }
}