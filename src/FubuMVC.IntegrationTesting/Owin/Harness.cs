using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Runtime;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [SetUpFixture]
    public class HarnessBootstrapper
    {
        [SetUp]
        public void SetUp()
        {
            Harness.Start();
        }

        [TearDown]
        public void TearDown()
        {
            Harness.Shutdown();
        }
    }

    public static class Harness
    {
        private static FubuRuntime _server;


        public static string Root
        {
            get { return _server.BaseAddress; }
        }

        public static void Start()
        {
            _server = FubuRuntime.Basic(_ =>
            {
                _.RootPath = GetRootDirectory();
                _.HostWith<Katana>();
            });
        }

        public static string GetRootDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory();
        }

        public static void Shutdown()
        {
            _server.Dispose();
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

        public static HttpResponse ShouldHaveHeaderValue(this HttpResponse response, string header, string value)
        {
            response.ResponseHeaderFor(header).ShouldBe(value);
            return response;
        }

        public static HttpResponse ContentShouldBe(this HttpResponse response, MimeType mimeType, string content)
        {
            response.ContentType.ShouldBe(mimeType.Value);
            response.ReadAsText().ShouldBe(content);

            return response;
        }

        public static HttpResponse ContentTypeShouldBe(this HttpResponse response, MimeType mimeType)
        {
            response.ContentType.ShouldBe(mimeType.Value);

            return response;
        }

        public static HttpResponse LengthShouldBe(this HttpResponse response, int length)
        {
            response.ContentLength().ShouldBe(length);

            return response;
        }

        public static HttpResponse ContentShouldBe(this HttpResponse response, string mimeType, string content)
        {
            response.ContentType.ShouldBe(mimeType);
            response.ReadAsText().ShouldBe(content);

            return response;
        }


        public static HttpResponse StatusCodeShouldBe(this HttpResponse response, HttpStatusCode code)
        {
            response.StatusCode.ShouldBe(code);

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