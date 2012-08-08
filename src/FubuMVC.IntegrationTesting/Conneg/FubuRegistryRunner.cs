using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Xml;
using FubuCore;
using FubuKayak;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.IntegrationTesting.Querying;
using FubuMVC.OwinHost;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.Conneg
{
    public abstract class FubuRegistryHarness
    {
        private Harness theHarness;

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
            runBottles("alias harness " + Harness.GetApplicationDirectory().FileEscape());

            runBottles("link harness --clean-all");
            runFubu("packages harness --clean-all --remove-all");

            initializeBottles();
            theHarness = Harness.Run(configure);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            theHarness.Dispose();
        }

        protected void restart()
        {
            TearDown();
            theHarness = Harness.Run(configure);
        }

        protected virtual void configure(FubuRegistry registry)
        {
        }

        protected virtual void initializeBottles()
        {
        }

        protected void runFubu(string commands)
        {
            var runner = new CommandRunner();
            commands.ReadLines(x =>
            {
                if (x.Trim().IsNotEmpty())
                {
                    runner.RunFubu(x);
                }
            });
        }

        protected void runBottles(string commands)
        {
            var runner = new CommandRunner();
            commands.ReadLines(x =>
            {
                if (x.Trim().IsNotEmpty())
                {
                    runner.RunBottles(x);
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

        public SimpleSource(Action<FubuRegistry> configuration)
        {
            _configuration = configuration;
        }

        public FubuApplication BuildApplication()
        {
            return FubuApplication.For(() =>
            {
                var registry = new FubuRegistry();
                registry.Actions.IncludeType<GraphQuery>();

                _configuration(registry);

                return registry;
            }).StructureMap(new Container());
        }
    }

    public class Harness : IDisposable
    {
        private static int _port = 5500;

        private readonly FubuKayakApplication _application;
        private readonly EndpointDriver _endpoints;
        private readonly Lazy<RemoteBehaviorGraph> _remote;
        private readonly FubuRuntime _runtime;

        public Harness(FubuRuntime runtime, FubuKayakApplication application, string root)
        {
            _runtime = runtime;
            _application = application;

            var urls = _runtime.Facility.Get<IUrlRegistry>();
            urls.As<UrlRegistry>().RootAt(root);

            UrlContext.Stub(root);

            _remote = new Lazy<RemoteBehaviorGraph>(() => { return new RemoteBehaviorGraph(root); });

            _endpoints = new EndpointDriver(urls);
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
            _application.Stop();
        }

        public static Harness Run(Action<FubuRegistry> configure)
        {
            var applicationDirectory = GetApplicationDirectory();
            FubuMvcPackageFacility.PhysicalRootPath = applicationDirectory;


            var application = new FubuKayakApplication(new SimpleSource(configure));
            var port = PortFinder.FindPort(_port++);

            var reset = new ManualResetEvent(false);
            FubuRuntime runtime = null;

            ThreadPool.QueueUserWorkItem(o =>
            {
                // Need to make this capture the package registry failures cleanly
                application.RunApplication(port, r =>
                {
                    runtime = r;
                    reset.Set();
                });
            });

            reset.WaitOne();

            var root = "http://localhost:" + port;

            return new Harness(runtime, application, root);


            // Need to get the runtime
        }

        public static string GetApplicationDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory();
        }
    }

    public static class HttpResponseExtensions
    {
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

        public static HttpResponse ShouldHaveHeader(this HttpResponse response, HttpResponseHeader header)
        {
            response.ResponseHeaderFor(header).ShouldNotBeEmpty();
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