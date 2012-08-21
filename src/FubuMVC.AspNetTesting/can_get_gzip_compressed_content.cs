using System.Net;
using AspNetApplication;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class can_get_gzip_compressed_content
    {
        [Test]
        public void get_the_content()
        {
            var response = TestApplication.Endpoints.GetByInput(new CompressedContentInput(), configure: request =>
            {
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
                request.AutomaticDecompression = DecompressionMethods.GZip;
            });

            var text = response.ReadAsText();
            text.ShouldStartWith("Hello, World!");
        }
    }
}