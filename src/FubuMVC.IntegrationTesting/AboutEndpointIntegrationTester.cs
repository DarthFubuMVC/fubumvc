﻿using System;
using System.Diagnostics;
using System.Threading;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http.Hosting;
using HtmlTags;
using Xunit;

namespace FubuMVC.IntegrationTesting
{
    
    public class AboutEndpointIntegrationTester : IDisposable
    {
        private readonly FubuRuntime server = FubuRuntime.Basic(_ =>
        {
            _.Mode = "development";
            _.HostWith<Katana>();
        });


        public void Dispose()
        {
            server.Dispose();
        }

        [Fact]
        public void can_get_The_about_page_smoke_test()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<AboutFubuDiagnostics>(x => x.get_about());
                _.ContentShouldContain("Assemblies");
            });
        }

    }


    public class ReloadingEndpoint
    {
        private readonly AppReloaded _reloaded;
        private readonly FubuRuntime _runtime;

        public ReloadingEndpoint(AppReloaded reloaded, FubuRuntime runtime)
        {
            _reloaded = reloaded;
            _runtime = runtime;
        }

        public HtmlDocument get_reloaded()
        {
            var document = new HtmlDocument();
            document.Title = "Manual Test Harness for reloading";
            document.Add("h1").Text("Loaded at " + _reloaded.Timestamp);

            document.Add(new AutoReloadingTag(_runtime.Mode));

            return document;
        }
    }
}