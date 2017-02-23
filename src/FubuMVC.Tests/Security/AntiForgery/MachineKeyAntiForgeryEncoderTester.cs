using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.AntiForgery
{
    
    public class MachineKeyAntiForgeryEncoderTester : InteractionContext<MachineKeyAntiForgeryEncoder>
    {
        [Fact]
        public void encoding_and_decoding_match()
        {
            var input = new byte[] {1, 2, 3, 4, 5};
            string encoded = ClassUnderTest.Encode(input);
            byte[] decoded = ClassUnderTest.Decode(encoded);

            decoded.ShouldBe(input);
        }
    }
}