using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class AssemblyNeedsTheFubuModuleAttribute
    {
        [Test]
        public void the_attribute_exists()
        {
            var assembly = typeof(AuthenticationFilter).Assembly;

            assembly.GetCustomAttributes(typeof(FubuModuleAttribute), true)
                .Any().ShouldBeTrue();
        }
    }
}