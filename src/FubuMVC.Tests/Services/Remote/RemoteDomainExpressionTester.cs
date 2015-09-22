using System;
using FubuCore;
using FubuMVC.Core.Services.Remote;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Services.Remote
{
    [TestFixture]
    public class RemoteDomainExpressionTester
    {
        private string serviceDirectory;

        [SetUp]
        public void SetUp()
        {
            serviceDirectory = Guid.NewGuid().ToString();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                new FileSystem().DeleteDirectory(serviceDirectory);
            }
            catch (Exception)
            {
                // Don't fail the build because of this.
            }
        }

        [Test]
        public void will_use_bin_for_private_bin_path_if_it_exists()
        {
            var fileSystem = new FileSystem();

           
            fileSystem.DeleteDirectory(serviceDirectory);

            fileSystem.CreateDirectory(serviceDirectory);
            fileSystem.CreateDirectory(serviceDirectory, "bin");

            var expression = new RemoteDomainExpression();
            expression.Setup.PrivateBinPath.ShouldBeNull();

            expression.ServiceDirectory = serviceDirectory;

            expression.Setup.PrivateBinPath.ShouldBe("bin");

            fileSystem.DeleteDirectory(serviceDirectory);
        }

        [Test]
        public void will_use_bin_release_for_private_bin_path_if_it_exists_and_release_has_precedence()
        {
            var fileSystem = new FileSystem();

            fileSystem.DeleteDirectory(serviceDirectory);

            fileSystem.CreateDirectory(serviceDirectory);
            fileSystem.CreateDirectory(serviceDirectory, "bin");
            fileSystem.CreateDirectory(serviceDirectory, "bin", "Release");
            fileSystem.CreateDirectory(serviceDirectory, "bin", "Debug");

            var expression = new RemoteDomainExpression();
            expression.Setup.PrivateBinPath.ShouldBeNull();

            expression.ServiceDirectory = serviceDirectory;

            expression.Setup.PrivateBinPath.ShouldBe("bin".AppendPath("Release"));

            fileSystem.DeleteDirectory(serviceDirectory);
        }


        [Test]
        public void will_use_bin_debug_for_private_bin_path_if_it_exists_and_release_does_not()
        {
            var fileSystem = new FileSystem();

            fileSystem.DeleteDirectory(serviceDirectory);

            fileSystem.CreateDirectory(serviceDirectory);
            fileSystem.CreateDirectory(serviceDirectory, "bin");
            fileSystem.CreateDirectory(serviceDirectory, "bin", "Debug");

            var expression = new RemoteDomainExpression();
            expression.Setup.PrivateBinPath.ShouldBeNull();

            expression.ServiceDirectory = serviceDirectory;

            expression.Setup.PrivateBinPath.ShouldBe("bin".AppendPath("Debug"));
        }

        [Test]
        public void do_not_use_bin_for_private_bin_path_if_it_does_not_exist()
        {
            var fileSystem = new FileSystem();

            fileSystem.DeleteDirectory("Service2");

            fileSystem.CreateDirectory("Service2");

            var expression = new RemoteDomainExpression();
            expression.Setup.PrivateBinPath.ShouldBeNull();

            expression.ServiceDirectory = "Service2";

            expression.Setup.PrivateBinPath.ShouldBeNull();
        }

        
    }
}