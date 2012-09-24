using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Http.Compression;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Http
{
    [TestFixture, Ignore("This does NOT work with Web API.  I think it's an issue on there side")]
    public class compression_testing : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<CompressionController>();
            registry.Import<ContentCompression>();
        }

        [Test]
        public void retrieves_the_gzip_compressed_content()
        {
            var response = endpoints.GetByInput(new CompressedInput(), acceptEncoding:"gzip");

            response.ResponseHeaderFor(HttpResponseHeader.ContentEncoding).ShouldEqual("gzip");
        }

        [Test]
        public void retrieves_the_deflate_compressed_content()
        {
            var response = endpoints.GetByInput(new CompressedInput(), acceptEncoding:"deflate");

            response.ResponseHeaderFor(HttpResponseHeader.ContentEncoding).ShouldEqual("deflate");
        }
    }

    public class CompressedInput { }

    public class CompressionController
    {
        public string get_compressed_content(CompressedInput input)
        {
            return "Hello, World!";
        }
    }
}