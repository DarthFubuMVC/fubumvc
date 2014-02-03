using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Katana;
using FubuMVC.OwinHost;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting
{
    [SetUpFixture]
    public class HarnessBootstrapper
    {
        [TearDown]
        public void TearDown()
        {
            SelfHostHarness.Shutdown();
        }
    }

    public static class SelfHostHarness
    {
        private static EmbeddedFubuMvcServer _server;

        public static void Start()
        {
            Recycle();
        }

        public static string GetRootDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory();
        }

        public static string Root
        {
            get
            {
                if (_server == null) Recycle();

                return _server.BaseAddress;
            }
        }

        public static EndpointDriver Endpoints
        {
            get
            {
                if (_server == null) Recycle();

                return _server.Endpoints;
            }
        }

        public static void Shutdown()
        {
            if (_server != null) _server.SafeDispose();
        }

        public static void Recycle()
        {
            if (_server != null)
            {
                _server.Dispose();
            }

            var port = PortFinder.FindPort(5500);
            var runtime = bootstrapRuntime();

            _server = new EmbeddedFubuMvcServer(runtime, GetRootDirectory(), port);
        }

        private static FubuRuntime bootstrapRuntime()
        {
            return FubuApplication.For<HarnessRegistry>().StructureMap(new Container()).Bootstrap();
        }
    }

    public class HarnessRegistry : FubuRegistry
    {

    }

    public static class HttpResponseExtensions
    {
        public static HttpResponse ShouldHaveHeader(this HttpResponse response, HttpResponseHeader header)
        {
            response.ResponseHeaderFor(header).ShouldNotBeEmpty();
            return response;
        }

        public static HttpResponse ContentShouldBe(this HttpResponse response, MimeType mimeType, string content)
        {
            response.ContentType.ShouldEqual(mimeType.Value);
            response.ReadAsText().ShouldEqual(content);

            return response;
        }

        public static HttpResponse ContentTypeShouldBe(this HttpResponse response, MimeType mimeType)
        {
            response.ContentType.ShouldEqual(mimeType.Value);

            return response;
        }

        public static HttpResponse LengthShouldBe(this HttpResponse response, int length)
        {
            response.ContentLength().ShouldEqual(length);

            return response;
        }

        public static HttpResponse ContentShouldBe(this HttpResponse response, string mimeType, string content)
        {
            response.ContentType.ShouldEqual(mimeType);
            response.ReadAsText().ShouldEqual(content);

            return response;
        }


        public static HttpResponse StatusCodeShouldBe(this HttpResponse response, HttpStatusCode code)
        {
            response.StatusCode.ShouldEqual(code);

            return response;
        }

        public static HttpResponse EtagShouldBe(this HttpResponse response, string etag)
        {
            etag.Trim('"').ShouldEqual(etag);
            return response;
        }

        public static DateTime? LastModified(this HttpResponse response)
        {
            var lastModifiedString = response.ResponseHeaderFor(HttpResponseHeader.LastModified);
            return lastModifiedString.IsEmpty() ? (DateTime?) null : DateTime.ParseExact(lastModifiedString, "r", null);
        }

        public static HttpResponse LastModifiedShouldBe(this HttpResponse response, DateTime expected)
        {
            var lastModified = response.LastModified();
            lastModified.HasValue.ShouldBeTrueBecause("No value for LastModified");
            lastModified.ShouldEqual(expected);

            return response;
        }

        public static string FileEscape(this string file)
        {
            return "\"{0}\"".ToFormat(file);
        }

        public static IEnumerable<string> ScriptNames(this HttpResponse response)
        {
            var document = response.ReadAsXml();
            var tags = document.DocumentElement.SelectNodes("//script");

            foreach (XmlElement tag in tags)
            {
                var name = tag.GetAttribute("src");
                yield return name.Substring(name.IndexOf('_'));
            }
        }
    }
}