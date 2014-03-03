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


}