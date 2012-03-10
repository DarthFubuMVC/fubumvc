using System;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuModeTester
    {
        [Test]
        public void DevMode_as_is_is_true()
        {
            Environment.SetEnvironmentVariable("FubuMode", "Development");
            FubuMode.Reset();
            FubuMode.InDevelopment().ShouldBeTrue();

            FubuMode.Mode().ShouldEqual("Development");
        }

        [Test]
        public void DevMode_is_false()
        {
            Environment.SetEnvironmentVariable("FubuMode", "Production");
            FubuMode.Reset();
            FubuMode.InDevelopment().ShouldBeFalse();

            FubuMode.Mode().ShouldEqual("Production");
        }

        [Test]
        public void override_devmode()
        {
            string isDev = FubuMode.Development;

            FubuMode.Mode(() => isDev);

            FubuMode.InDevelopment().ShouldBeTrue();

            isDev = "something else";

            FubuMode.Reset();
            FubuMode.InDevelopment().ShouldBeFalse();
        }
    }
}