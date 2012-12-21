using System;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuModeTester
    {
        [SetUp]
        public void SetUp()
        {
            FubuMode.Reset();
        }

        [Test]
        public void DevMode_as_is_is_true()
        {
            FubuMode.Detector.SetMode("Development");
            FubuMode.InDevelopment().ShouldBeTrue();

            FubuMode.Mode().ShouldEqual("Development");
        }

        [Test]
        public void DevMode_is_false()
        {
            FubuMode.Detector.SetMode("Production");
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

            FubuMode.Detector = new EnvironmentVariableDetector();
            FubuMode.Detector.SetMode("");

            FubuMode.InDevelopment().ShouldBeFalse();
        }

        [Test]
        public void fubu_mode_should_default_to_false_if_environment_doesnt_exist()
        {
            FubuMode.Detector.SetMode("");
            FubuMode.InDevelopment().ShouldBeFalse();
        }



        [Test]
        public void DevMode_as_is_is_true_with_file()
        {
            Environment.SetEnvironmentVariable("FubuMode", "", EnvironmentVariableTarget.Machine);

            FubuMode.Detector = new FubuModeFileDetector();

            FubuMode.Detector.SetMode("Development");

            FubuMode.InDevelopment().ShouldBeTrue();

            FubuMode.Mode().ShouldEqual("Development");
        }

        [Test]
        public void DevMode_is_false_with_file()
        {
            Environment.SetEnvironmentVariable("FubuMode", "Development", EnvironmentVariableTarget.Machine);
            
            FubuMode.Detector = new FubuModeFileDetector();
            FubuMode.Detector.SetMode("Production");

            FubuMode.InDevelopment().ShouldBeFalse();

            FubuMode.Mode().ShouldEqual("Production");
        }

        [Test]
        public void fubu_mode_should_default_to_false_if_environment_doesnt_exist_with_file()
        {
            FubuMode.Detector = new FubuModeFileDetector();
            FubuModeFileDetector.Clear();

            FubuMode.InDevelopment().ShouldBeFalse();
        }
    }
}