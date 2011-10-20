using FubuMVC.Core.Http;
using NUnit.Framework;

namespace FubuMVC.Tests.Http
{
    [TestFixture, Ignore("Come back to this")]
    public class CookieTester
    {
        private Cookie theCookie;

        [SetUp]
        public void SetUp()
        {
            theCookie = new Cookie();
        }

        [Test]
        public void get_header_value_with_a_single_value()
        {
            
        }
    }
}