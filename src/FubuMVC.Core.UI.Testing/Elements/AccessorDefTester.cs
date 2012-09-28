using FubuMVC.Core.UI.Elements;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Core.UI.Testing.Elements
{
    [TestFixture]
    public class AccessorDefTester
    {
        [Test]
        public void is_positive()
        {
            var def = AccessorDef.For<Address>(x => x.Address1);

            def.Is<string>().ShouldBeTrue();
        }

        [Test]
        public void is_negative()
        {
            AccessorDef.For<Address>(x => x.Address1).Is<bool>().ShouldBeFalse();
        }
    }
}