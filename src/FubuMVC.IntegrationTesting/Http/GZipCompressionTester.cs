using System.IO.Compression;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Compression;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Http
{
    [TestFixture]
    public class GZipCompressionTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<CompressionController>();
            registry.Import<ContentCompression>();
        }

        [Test]
        public void retrieves_the_compressed_content()
        {
            var response = endpoints.GetByInput(new CompressedInput(), configure: request =>
            {
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
            });

            response.ResponseHeaderFor(HttpResponseHeader.ContentEncoding).ShouldEqual("gzip");

            var output = response.Output();
            using(var stream = new GZipStream(output, CompressionMode.Decompress))
            {
                var text = stream.ReadAllText();
                text.ShouldNotBeEmpty();
            }
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