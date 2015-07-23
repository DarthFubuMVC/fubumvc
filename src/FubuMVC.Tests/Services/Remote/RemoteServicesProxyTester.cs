using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Services;
using FubuMVC.Core.Services.Remote;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Services.Remote
{
    [TestFixture]
    public class RemoteServicesProxyTester
    {
        [Test]
        public void set_properties_on_PackageRegistry_before_loading_the_loader()
        {
            SimpleLoader.Color = null;

            new RemoteServicesProxy().Start(typeof(SimpleLoader).AssemblyQualifiedName, new Dictionary<string, string>{{"Color", "Fuschia"}}, null );

            SimpleLoader.Color.ShouldBe("Fuschia");
        }
    }

    public class SimpleLoader : IApplicationLoader, IDisposable
    {
        public static string Color;

        public IDisposable Load()
        {
            Color = FubuApplication.Properties["Color"];
            return this;
        }

        public void Dispose()
        {
            
        }
    }
}