using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Xml;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using Owin;
using StructureMap;

namespace FubuMVC.OwinHost.Testing
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

    public class HarnessApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<HarnessRegistry>().StructureMap(Harness.Container);
        }
    }

    public static class Harness
    {
        private static IDisposable _server;
        private static EndpointDriver _endpoints;
        private static string _root;

        public static IContainer Container { get; set; }

        public static void Start()
        {
            FubuMvcPackageFacility.PhysicalRootPath = GetRootDirectory();
            Container = new Container();

            var port = PortFinder.FindPort(5501);

            FubuRuntime runtime = null;
            _server = WebApplication.Start<Starter>(port: port, verbosity:1);

            _root = "http://localhost:" + port;

            var urls = Container.GetInstance<IUrlRegistry>();
            urls.As<UrlRegistry>().RootAt(_root);

            UrlContext.Stub(_root);

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
                return _root;
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
            _server.Dispose();
            Container = null;
        }
    }

    public class Starter
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.RunFubu<HarnessApplication>();
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