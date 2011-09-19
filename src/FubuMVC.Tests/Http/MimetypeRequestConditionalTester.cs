using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class MimetypeRequestConditionalTester
    {
        [Test]
        public void matches_negative()
        {
            var conditional = new MimetypeRequestConditional("text/json", "application/json");
            conditional.Matches("text/plain").ShouldBeFalse();
        }

        [Test]
        public void matches_positive()
        {
            var conditional = new MimetypeRequestConditional("text/json", "application/json");
            conditional.Matches("text/json").ShouldBeTrue();
        }
    }
}