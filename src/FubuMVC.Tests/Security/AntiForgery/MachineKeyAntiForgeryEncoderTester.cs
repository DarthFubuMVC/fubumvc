using FubuMVC.Core.Security.AntiForgery;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.AntiForgery
{
	[TestFixture]
	public class MachineKeyAntiForgeryEncoderTester : InteractionContext<MachineKeyAntiForgeryEncoder>
	{

		[Test]
		public void encoding_and_decoding_match()
		{
			var input = new byte[]{1,2,3,4,5};
			var encoded = ClassUnderTest.Encode(input);
			var decoded = ClassUnderTest.Decode(encoded);

			decoded.ShouldEqual(input);
		}


	}
}