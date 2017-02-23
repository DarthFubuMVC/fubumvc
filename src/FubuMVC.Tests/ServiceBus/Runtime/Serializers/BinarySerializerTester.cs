using System.IO;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Serializers
{
    
    public class BinarySerializerTester
    {
        [Fact]
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
            address1.ShouldBe(address2);
        }
    }
}