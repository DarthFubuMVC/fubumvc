using FubuCore;
using FubuMVC.Core.Services.Remote;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Services.Remote
{
    [TestFixture]
    public class RemoteDomainExpressionTester
    {
        [Test]
        public void will_use_bin_for_private_bin_path_if_it_exists()
        {
            var fileSystem = new FileSystem();

            fileSystem.DeleteDirectory("Service");

            fileSystem.CreateDirectory("Service");
            fileSystem.CreateDirectory("Service", "bin");

            var expression = new RemoteDomainExpression();
            expression.Setup.PrivateBinPath.ShouldBeNull();

            expression.ServiceDirectory = "Service";

            expression.Setup.PrivateBinPath.ShouldBe("bin");
        }

        [Test]
        public void will_use_bin_release_for_private_bin_path_if_it_exists_and_release_has_precedence()
        {
            var fileSystem = new FileSystem();

            fileSystem.DeleteDirectory("Service");

            fileSystem.CreateDirectory("Service");
            fileSystem.CreateDirectory("Service", "bin");
            fileSystem.CreateDirectory("Service", "bin", "Release");
            fileSystem.CreateDirectory("Service", "bin", "Debug");

            var expression = new RemoteDomainExpression();
            expression.Setup.PrivateBinPath.ShouldBeNull();

            expression.ServiceDirectory = "Service";

            expression.Setup.PrivateBinPath.ShouldBe("bin".AppendPath("Release"));
        }


        [Test]
        public void will_use_bin_debug_for_private_bin_path_if_it_exists_and_release_does_not()
        {
            var fileSystem = new FileSystem();

            fileSystem.DeleteDirectory("Service");

            fileSystem.CreateDirectory("Service");
            fileSystem.CreateDirectory("Service", "bin");
            fileSystem.CreateDirectory("Service", "bin", "Debug");

            var expression = new RemoteDomainExpression();
            expression.Setup.PrivateBinPath.ShouldBeNull();

            expression.ServiceDirectory = "Service";

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