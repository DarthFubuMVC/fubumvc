using FubuMVC.Core.Rest.Media.Formatters;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Rest.Media.Formatters
{
    [TestFixture]
    public class JsonFormatterTester : InteractionContext<JsonFormatter>
    {
        [Test]
        public void has_the_right_mime_types()
        {
            ClassUnderTest.MatchingMimetypes.ShouldHaveTheSameElementsAs("application/json", "text/json");
        }
    }

    [TestFixture]
    public class XmlFormatterTester : InteractionContext<XmlFormatter>
    {
        [Test]
        public void has_the_right_mime_types()
        {
            ClassUnderTest.MatchingMimetypes.ShouldHaveTheSameElementsAs("text/xml", "application/xml");
        }
    }
}