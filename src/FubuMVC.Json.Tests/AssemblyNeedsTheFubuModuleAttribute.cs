using System.Linq;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Json.Tests
{
	[TestFixture]
	public class AssemblyNeedsTheFubuModuleAttribute
	{
		[Test]
		public void the_attribute_exists()
		{
			var assembly = typeof(NewtonsoftJsonFormatter).Assembly;

			assembly.GetCustomAttributes(typeof(FubuModuleAttribute), true)
				.Any().ShouldBeTrue();
		}
	}
}