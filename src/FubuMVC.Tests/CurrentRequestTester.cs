using FubuMVC.Core;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class CurrentRequestTester
    {
        [Test]
        public void get_the_requested_mime_type_if_content_type_is_null()
        {
            var request = new CurrentRequest();
            request.RequestedMimeType().ShouldBeEmpty();
        }

        [Test]
        public void get_the_requested_mime_type_if_content_type_is_simple()
        {
            var request = new CurrentRequest(){
                ContentType = "text/xml"
            };

            request.RequestedMimeType().ShouldEqual("text/xml");
        }

        [Test]
        public void get_the_requested_mime_type_if_the_content_type_has_the_charset_as_well()
        {
            var request = new CurrentRequest()
            {
                ContentType = "text/xml; charset something or other"
            };

            request.RequestedMimeType().ShouldEqual("text/xml");
        }
    }
}