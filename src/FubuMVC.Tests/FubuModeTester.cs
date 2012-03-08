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
            FubuMode.DevModeReset();
            FubuMode.DevMode().ShouldBeTrue();
        }

        [Test]
        public void DevMode_is_false()
        {
            Environment.SetEnvironmentVariable("FubuMode", "Production");
            FubuMode.DevModeReset();
            FubuMode.DevMode().ShouldBeFalse();
        }

        [Test]
        public void override_devmode()
        {
            bool isDev = true;

            FubuMode.DevMode(() => isDev);

            FubuMode.DevMode().ShouldBeTrue();

            isDev = false;

            FubuMode.DevModeReset();
            FubuMode.DevMode().ShouldBeFalse();
        }
    }
}