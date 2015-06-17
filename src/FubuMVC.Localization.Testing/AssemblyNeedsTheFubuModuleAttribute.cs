using System.Linq;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Localization.Testing
{
    [TestFixture]
    public class AssemblyNeedsTheFubuModuleAttribute
    {
        [Test]
        public void the_attribute_exists()
        {
            var assembly = typeof(BasicLocalizationSupport).Assembly;

            assembly.GetCustomAttributes(typeof(FubuModuleAttribute), true)
                .Any().ShouldBeTrue();
        }
    }
}