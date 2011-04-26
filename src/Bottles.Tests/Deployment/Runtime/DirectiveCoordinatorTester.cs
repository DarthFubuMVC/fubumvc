using System.Collections.Generic;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment.Runtime
{
    [TestFixture]
    public class DirectiveCoordinatorTester : InteractionContext<DirectiveCoordinator>
    {
        private IEnumerable<HostManifest> _hosts;
        private FakeDirective _directive;
        private IDeploymentDiagnostics _deploymentDiagnostics;
        private IEnumerable<IInitializer<FakeDirective>> _initializers;
        private FakeInitializer<FakeDirective> _init;
        private FakeFinalizer<FakeDirective> _fin;
        private FakeDeployer<FakeDirective> _dep;
        private List<FakeFinalizer<FakeDirective>> _finalizers;
        private List<FakeDeployer<FakeDirective>> _deployers;

        protected override void beforeEach()
        {
            _directive = new FakeDirective();

            var a  = DataMother.BuildHostManifest("name", _directive);
            
            _hosts = new List<HostManifest>{a};

            _deploymentDiagnostics = new FakeDeploymentDiagnostics();
            _init = new FakeInitializer<FakeDirective>();
            _fin = new FakeFinalizer<FakeDirective>();
            _dep = new FakeDeployer<FakeDirective>();

            _initializers = new List<FakeInitializer<FakeDirective>>() {_init};
            _finalizers = new List<FakeFinalizer<FakeDirective>>  {_fin};
            _deployers = new List<FakeDeployer<FakeDirective>> {_dep};


            MockFor<ICommandFactory>()
                .Stub(f=>f.InitializersFor(_directive))
                .Return(new InitializerSet<FakeDirective>(_deploymentDiagnostics, _initializers));

            MockFor<ICommandFactory>()
                .Stub(f => f.DeployersFor(_directive))
                .Return(new DeployerSet<FakeDirective>(_deploymentDiagnostics, _deployers));

            MockFor<ICommandFactory>()
                .Stub(f => f.FinalizersFor(_directive))
                .Return(new FinalizerSet<FakeDirective>(_deploymentDiagnostics, _finalizers));
            
        }

        [Test]
        public void Initializers()
        {
            ClassUnderTest.Initialize(_hosts);

            MockFor<ICommandFactory>().AssertWasCalled(f=>f.InitializersFor(_directive));

            _directive.Hits.ShouldEqual(1);
        }

        [Test]
        public void Deployers()
        {
            ClassUnderTest.Deploy(_hosts);

            MockFor<ICommandFactory>().AssertWasCalled(f => f.DeployersFor(_directive));

            _directive.Hits.ShouldEqual(1);
        }

        [Test]
        public void Finalizers()
        {
            ClassUnderTest.Finish(_hosts);

            MockFor<ICommandFactory>().AssertWasCalled(f => f.FinalizersFor(_directive));

            _directive.Hits.ShouldEqual(1);
        }
    }
}