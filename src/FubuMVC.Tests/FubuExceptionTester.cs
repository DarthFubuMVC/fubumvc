using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuExceptionTester
    {
        [Test]
        public void can_transfer_fubuexception_across_boundaries()
        {
            var original = new FubuException(42, "The answer to {0}, the {1} and {2}", "life", "universe", "everything");
            var transferred = TransferViaSerialization(original);
            transferred.ErrorCode.ShouldEqual(original.ErrorCode);
            transferred.Message.ShouldEqual(original.Message);
        }

        [Test]
        public void can_transfer_fubuassertionexception_across_boundaries()
        {
            var original = new FubuAssertionException("Nothing works");
            var transferred = TransferViaSerialization(original);
            transferred.Message.ShouldEqual(original.Message);
        }


        public static T TransferViaSerialization<T>(T instance)
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, instance);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }

    }
}