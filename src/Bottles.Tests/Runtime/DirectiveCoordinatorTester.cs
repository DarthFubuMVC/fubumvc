using System.Collections.Generic;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Runtime;
using Bottles.Tests.Deployment.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Runtime
{
    [TestFixture]
    public class DirectiveCoordinatorTester : InteractionContext<DirectiveCoordinator>
    {
        private IEnumerable<HostManifest> _hosts;
        private FakeDirective _directive;
        private ILogger _logger;
        private IEnumerable<IInitializer<FakeDirective>> _initializers;
        private FakeInitializer<FakeDirective> _init;

        protected override void beforeEach()
        {
            _directive = new FakeDirective();

            var a = new HostManifest("bob");
            
            //add directive

            _hosts = new List<HostManifest>{a};

            _logger = new FakeLogger();
            _init = new FakeInitializer<FakeDirective>();
            _initializers = new List<FakeInitializer<FakeDirective>>() {_init};


            MockFor<ICommandFactory>()
                .Stub(f=>f.InitializersFor(_directive))
                .Return(new InitializerSet<FakeDirective>(_logger, _initializers));
            
        }

        [Test]
        public void a()
        {
            ClassUnderTest.Initialize(_hosts);

            MockFor<ICommandFactory>().AssertWasCalled(f=>f.InitializersFor(_directive));

            _directive.Hits.ShouldEqual(1);
        }
    }
}