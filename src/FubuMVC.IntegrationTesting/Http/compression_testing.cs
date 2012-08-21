﻿using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Http.Compression;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Http
{
    [TestFixture]
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
            var response = endpoints.GetByInput(new CompressedInput(), configure: request =>
            {
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
            });

            response.ResponseHeaderFor(HttpResponseHeader.ContentEncoding).ShouldEqual("gzip");
        }

        [Test]
        public void retrieves_the_deflate_compressed_content()
        {
            var response = endpoints.GetByInput(new CompressedInput(), configure: request =>
            {
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "deflate");
            });

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