using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.AntiForgery.Testing
{
    [TestFixture]
    public class MachineKeyAntiForgeryEncoderTester : InteractionContext<MachineKeyAntiForgeryEncoder>
    {
        [Test]
        public void encoding_and_decoding_match()
        {
            var input = new byte[] {1, 2, 3, 4, 5};
            string encoded = ClassUnderTest.Encode(input);
            byte[] decoded = ClassUnderTest.Decode(encoded);

            decoded.ShouldEqual(input);
        }
    }
}