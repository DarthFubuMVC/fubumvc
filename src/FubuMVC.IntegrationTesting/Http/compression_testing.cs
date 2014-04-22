using System.Net;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Http
{
    [TestFixture, Ignore("Broken in 2.0")]
    public class compression_testing : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<CompressionController>();
            registry.Policies.Local.Add(policy => policy.ContentCompression.Apply());
        }

        [Test]
        public void retrieves_the_gzip_compressed_content()
        {
            var response = endpoints.GetByInput(new CompressedInput(), acceptEncoding: "gzip");

            response.ResponseHeaderFor(HttpResponseHeader.ContentEncoding).ShouldEqual("gzip");
        }

        [Test]
        public void retrieves_the_deflate_compressed_content()
        {
            var response = endpoints.GetByInput(new CompressedInput(), acceptEncoding: "deflate");

            response.ResponseHeaderFor(HttpResponseHeader.ContentEncoding).ShouldEqual("deflate");
        }
    }

    public class CompressedInput
    {
    }

    public class CompressionController
    {
        public string get_compressed_content(CompressedInput input)
        {
            return "Hello, World!";
        }
    }
}