using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.SelfHost;
using FubuMVC.StructureMap;
using NUnit.Framework;
using StructureMap;
using FubuTestingSupport;

namespace FubuMVC.Core.Assets.IntegrationTesting
{
    [SetUpFixture]
    public class HarnessBootstrapper
    {
        [SetUp]
        public void SetUp()
        {
            SelfHostHarness.Start();
        }

        [TearDown]
        public void TearDown()
        {
            SelfHostHarness.Shutdown();
        }
    }

    public static class SelfHostHarness
    {
        private static SelfHostHttpServer _server;
        private static EndpointDriver _endpoints;

        public static void Start()
        {
            _server = new SelfHostHttpServer(5501);
            var runtime = FubuApplication.For<HarnessRegistry>().StructureMap(new Container()).Bootstrap();
        
        
            _server.Start(runtime, GetRootDirectory());

            var urls = runtime.Facility.Get<IUrlRegistry>();
            urls.As<UrlRegistry>().RootAt(_server.BaseAddress);

            UrlContext.Stub(_server.BaseAddress);

            _endpoints = new EndpointDriver(urls);
        }

        public static string GetRootDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory();
        }

        public static string Root
        {
            get
            {
                return _server.BaseAddress;
            }
        }

        public static EndpointDriver Endpoints
        {
            get
            {
                return _endpoints;
            }
        }

        public static void Shutdown()
        {
            _server.SafeDispose();
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