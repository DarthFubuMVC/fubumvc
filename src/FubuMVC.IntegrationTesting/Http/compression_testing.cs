using FubuMVC.Core;
using FubuMVC.Core.Http;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Http
{
    [TestFixture]
    public class compression_testing
    {
        [Test]
        public void retrieves_the_gzip_compressed_content()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Input<CompressedInput>();

                _.Request.AppendHeader(HttpRequestHeaders.AcceptEncoding, "gzip");

                _.Header(HttpGeneralHeaders.ContentEncoding).SingleValueShouldEqual("gzip");
            });
        }

        [Test]
        public void retrieves_the_deflate_compressed_content()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Input<CompressedInput>();

                _.Request.AppendHeader(HttpRequestHeaders.AcceptEncoding, "deflate");

                _.Header(HttpGeneralHeaders.ContentEncoding).SingleValueShouldEqual("deflate");
            });
        }
    }

    public class CompressedInput
    {
    }

    public class CompressionEndpoint
    {
        [CompressContent]
        public string get_compressed_content(CompressedInput input)
        {
            return "Hello, World!";
        }
    }
}