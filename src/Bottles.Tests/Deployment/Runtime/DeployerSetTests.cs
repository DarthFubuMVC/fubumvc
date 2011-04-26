using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment.Runtime
{
    [TestFixture]
    public class DeployerSetTests : InteractionContext<DeployerSet<FakeDirective>>
    {
        FakeDirective _directive = new FakeDirective();

        [Test]
        public void TestInitialization()
        {
            MockFor<IDeploymentDiagnostics>();

            HostManifest hostManifest = new HostManifest("bob");

            ClassUnderTest.Process(hostManifest, _directive);

            MockFor<IDeploymentDiagnostics>()
                .Expect(d => d.LogDeployer(null, hostManifest, null)).IgnoreArguments();

            MockFor<IDeployer<FakeDirective>>().Expect(i => i.Deploy(_directive));

        }
    }
}