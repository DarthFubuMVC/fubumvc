using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using Bottles.Commands;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime;
using FubuMVC.IntegrationTesting.Querying;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting
{
    public abstract class FubuRegistryHarness
    {
        private Harness theHarness;
        private IContainer theContainer;

        private static readonly CommandRunner _runner = new CommandRunner();


        static FubuRegistryHarness()
        {
            new AliasCommand().Execute(new AliasInput{
                Name = "harness",
                Folder = Harness.GetApplicationDirectory().FileEscape()
            });
        }

        public RemoteBehaviorGraph remote
        {
            get { return theHarness.Remote; }
        }

        protected EndpointDriver endpoints
        {
            get { return theHarness.Endpoints; }
        }

        public string BaseAddress
        {
            get { return theHarness.BaseAddress; }
        }


        [TestFixtureSetUp]
        public void SetUp()
        {
            beforeRunning();

            initializeBottles();

            theContainer = new Container();
            configureContainer(theContainer);

            theHarness = Harness.Run(configure, theContainer);
        }

        protected void installZipPackage(string zipFile)
        {
            _runner.InstallZipPackage(zipFile);
        }

        protected void uninstallZipPackage(string zipFile)
        {
            _runner.UnInstallZipPackage(zipFile);
        }

        protected void removeAllLinkedPackages()
        {
            _runner.RemoveAllLinks();
        }

        protected void cleanAndRemoveAllPackages()
        {
            _runner.CleanAndRemoveAllPackages();
        }

        protected virtual void beforeRunning()
        {
            
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            afterRunning();
            theHarness.Dispose();
        }

        protected virtual void afterRunning()
        {
        }

        protected void restart()
        {
            TearDown();
            theHarness = Harness.Run(configure, theContainer);
        }

        protected virtual void configureContainer(IContainer container)
        {
        }

        protected virtual void configure(FubuRegistry registry)
        {
        }

        protected virtual void initializeBottles()
        {
        }

        protected static void runBottles(string commands)
        {
            commands.ReadLines(x =>
            {
                if (x.Trim().IsNotEmpty())
                {
                    _runner.RunBottles(x);
                }
            });
        }

        protected void DebugRemoteBehaviorGraph()
        {
            throw new NotImplementedException("Redo this");
            //var output = endpoints.Get<BehaviorGraphWriter>(x => x.PrintRoutes());
            //Debug.WriteLine(output);
        }

        protected void DebugPackageLoading()
        {
            throw new NotImplementedException("Redo this");
            //var output = endpoints.Get<PackageLoadingWriter>(x => x.FullLog());
            //var filename = Path.GetTempFileName() + ".htm";

            //new FileSystem().WriteStringToFile(filename, output.ToString());

            //Process.Start(filename);
        }
    }


    public class SimpleSource : IApplicationSource
    {
        private readonly Action<FubuRegistry> _configuration;
        private readonly IContainer _container; 

        public SimpleSource(Action<FubuRegistry> configuration, IContainer container)
        {
            _configuration = configuration;
            _container = container;
        }

        public FubuApplication BuildApplication()
        {
            return FubuApplication.For(() =>
            {
                var registry = new FubuRegistry();
                registry.Actions.IncludeType<GraphQuery>();

                _configuration(registry);

                return registry;
            }).StructureMap(_container);
        }
    }

    public class Harness : IDisposable
    {
        private readonly Lazy<RemoteBehaviorGraph> _remote;
        private static int _port = 5550;
        private readonly EmbeddedFubuMvcServer _server;

        public Harness(FubuRuntime runtime, int port)
        {
            _port = PortFinder.FindPort(port);
            _server = new EmbeddedFubuMvcServer(runtime, GetApplicationDirectory(), _port);
            _port = port;

            _remote = new Lazy<RemoteBehaviorGraph>(() =>
            {
                return new RemoteBehaviorGraph(_server.BaseAddress);
            });
        }

        public string BaseAddress
        {
            get { return _server.BaseAddress; }
        }

        public static int Port
        {
            get { return _port; }
        }

        public EndpointDriver Endpoints
        {
            get { return _server.Endpoints; }
        }

        public RemoteBehaviorGraph Remote
        {
            get { return _remote.Value; }
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        public static Harness Run(Action<FubuRegistry> configure, IContainer container)
        {
            var applicationDirectory = GetApplicationDirectory();
            FubuMvcPackageFacility.PhysicalRootPath = applicationDirectory;


            var simpleSource = new SimpleSource(configure, container);
            var runtime = simpleSource.BuildApplication().Bootstrap();

            var harness = new Harness(runtime, PortFinder.FindPort(_port++));

            _port = Port + 1;
            return harness;
        }

        public static string GetApplicationDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory();
        }
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



        public static HttpResponse EtagShouldBe(this HttpResponse response, string etag)
        {
            etag.Trim('"').ShouldEqual(etag);
            return response;
        }

        public static DateTime? LastModified(this HttpResponse response)
        {
            var lastModifiedString = response.ResponseHeaderFor(HttpResponseHeader.LastModified);
            return lastModifiedString.IsEmpty() ? (DateTime?)null : DateTime.ParseExact(lastModifiedString, "r", null);
        }

        public static HttpResponse LastModifiedShouldBe(this HttpResponse response, DateTime expected)
        {
            var lastModified = response.LastModified();
            lastModified.HasValue.ShouldBeTrueBecause("No value for LastModified");
            lastModified.ShouldEqual(expected);

            return response;
        }


    }
}