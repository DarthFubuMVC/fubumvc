using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class FubuExceptionTester
    {
        [Test]
        public void can_transfer_fubuexception_across_boundaries()
        {
            var original = new FubuException(42, "The answer to {0}, the {1} and {2}", "life", "universe", "everything");
            var transferred = original.ShouldTransferViaSerialization();
            transferred.ErrorCode.ShouldEqual(original.ErrorCode);
            transferred.Message.ShouldEqual(original.Message);
        }

        [Test]
        public void can_transfer_fubuassertionexception_across_boundaries()
        {
            var original = new FubuAssertionException("Nothing works");
            var transferred = original.ShouldTransferViaSerialization();
            transferred.Message.ShouldEqual(original.Message);
        }



    }
}