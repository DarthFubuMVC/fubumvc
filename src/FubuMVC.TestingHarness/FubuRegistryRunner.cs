using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Xml;
using Bottles.Commands;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.SelfHost;
using FubuMVC.StructureMap;
using FubuMVC.TestingHarness.Querying;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.TestingHarness
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


        [TestFixtureSetUp]
        public void SetUp()
        {
            beforeRunning();

            removeAllLinkedPackages();

            cleanAndRemoveAllPackages();

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

        public string BaseAddress
        {
            get { return theHarness.BaseAddress; }
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
        private readonly EndpointDriver _endpoints;
        private readonly Lazy<RemoteBehaviorGraph> _remote;
        private readonly FubuRuntime _runtime;
        private readonly SelfHostHttpServer _server;
        private static int _port = 5502;

        public Harness(FubuRuntime runtime, int port)
        {
            _runtime = runtime;

            _server = new SelfHostHttpServer(port);
            _server.Start(runtime, GetApplicationDirectory());
            _port = _server.Port;

            var urls = _runtime.Facility.Get<IUrlRegistry>();
            urls.As<UrlRegistry>().RootAt(_server.BaseAddress);

            UrlContext.Stub(_server.BaseAddress);

            _remote = new Lazy<RemoteBehaviorGraph>(() =>
            {
                return new RemoteBehaviorGraph(_server.BaseAddress);
            });

            _endpoints = new EndpointDriver(urls);
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
            get { return _endpoints; }
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
    }
}