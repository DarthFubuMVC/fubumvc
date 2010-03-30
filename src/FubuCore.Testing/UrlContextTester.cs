using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class UrlContextTester
    {
        [SetUp]
        public void SetUp()
        {
            UrlContext.Stub();
        }

        [Test]
        public void get_url()
        {
            UrlContext.GetUrl("someUrl").ShouldEqual("/someUrl");
        }

        [Test]
        public void get_full_url()
        {
            UrlContext.GetFullUrl("~SomePath").ShouldEqual("/SomePath");
        }

        [Test]
        public void map_path()
        {
            UrlContext.MapPath("~SomePath").ShouldEqual("/SomePath");
        }

        [Test]
        public void physical_path()
        {
            UrlContext.PhysicalPath("~/SomePath").ShouldEqual("\\SomePath");
        }
    }
}