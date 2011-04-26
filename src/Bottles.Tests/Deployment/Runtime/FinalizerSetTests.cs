using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment.Runtime
{
    [TestFixture]
    public class FinalizerSetTests : InteractionContext<FinalizerSet<FakeDirective>>
    {
        FakeDirective _directive = new FakeDirective();

        [Test]
        public void TestInitialization()
        {
            MockFor<IDeploymentDiagnostics>();

            HostManifest hostManifest = new HostManifest("bob");

            ClassUnderTest.Process(hostManifest, _directive);

            MockFor<IDeploymentDiagnostics>()
                .Expect(d => d.LogFinalizer(null, hostManifest, null)).IgnoreArguments();

            MockFor<IFinalizer<FakeDirective>>().Expect(i => i.Finish(_directive));

        }
    }
}