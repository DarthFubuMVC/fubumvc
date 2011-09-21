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

        [Test]
        public void writes_with_the_correct_mimetype_passed_into_it()
        {
            Assert.Fail("do.");
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

        [Test]
        public void writes_the_correct_mimetype_passed_into_it()
        {
            Assert.Fail("do.");
        }
    }
}