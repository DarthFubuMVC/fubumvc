using System;
using AspNetApplication;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Urls;
using FubuMVC.IntegrationTesting;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuMVC.AspNetTesting
{
    public static class TestApplication
    {
        private static readonly Lazy<FubuRuntime> _runtime = new Lazy<FubuRuntime>(() =>
        {
            var runner = new CommandRunner();
            runner.RunFubu("createvdir src/AspNetApplication fubumvc_aspnet_testing");

            return FubuApplication.For<AspNetApplicationFubuRegistry>().StructureMap(new Container()).Bootstrap();
        });

        private static readonly Lazy<IUrlRegistry> _urls = new Lazy<IUrlRegistry>(() =>
        {
            var urls = _runtime.Value.Facility.Get<IUrlRegistry>();
            urls.As<UrlRegistry>().RootAt("http://localhost/fubumvc_aspnet_testing");

            return urls;
        });

        private static readonly Lazy<EndpointDriver> _endpoints =
            new Lazy<EndpointDriver>(() => { return new EndpointDriver(_urls.Value); });

        public static FubuRuntime Runtime
        {
            get { return _runtime.Value; }
        }

        public static EndpointDriver Endpoints
        {
            get { return _endpoints.Value; }
        }

        public static IUrlRegistry Urls
        {
            get { return _urls.Value; }
        }

        public static void DebugRemoteBehaviorGraph()
        {
            throw new NotImplementedException("Redo.");
            //var output = Endpoints.Get<BehaviorGraphWriter>(x => x.PrintRoutes());
            //Debug.WriteLine(output);
        }

        public static void DebugPackageLoading()
        {
            throw new NotImplementedException("Redo.");
            //var output = Endpoints.Get<PackageLoadingWriter>(x => x.FullLog());
            //var filename = Path.GetTempFileName() + ".htm";

            //new FileSystem().WriteStringToFile(filename, output.ToString());

            //Process.Start(filename);
        }
    }
}