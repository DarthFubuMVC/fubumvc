using System;
using Bottles;
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

        [Test]
        public void in_testing_mode_if_package_registry_is_set()
        {
            PackageRegistry.Properties[FubuMode.Testing] = true.ToString();

            FubuMode.InTestingMode().ShouldBeTrue();
        }

        [Test]
        public void in_testing_mode_is_false()
        {
            PackageRegistry.Properties[FubuMode.Testing] = false.ToString();

            FubuMode.InTestingMode().ShouldBeFalse();
        }

        [Test]
        public void in_testing_mode_is_false_if_no_property_exists()
        {
            PackageRegistry.Properties.Remove(FubuMode.Testing);

            FubuMode.InTestingMode().ShouldBeFalse();
        }

        [Test]
        public void setup_for_testing_mode()
        {
            PackageRegistry.Properties.Remove(FubuMode.Testing);
            FubuMode.SetupForTestingMode();
            FubuMode.InTestingMode().ShouldBeTrue();
        }

        [Test]
        public void remote_testing_mode()
        {
            FubuMode.SetupForTestingMode();
            FubuMode.RemoveTestingMode();

            FubuMode.InTestingMode().ShouldBeFalse();
        }
    }
}