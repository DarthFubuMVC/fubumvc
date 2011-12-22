using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.SparkModel.Sharing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Sharing
{
    [TestFixture]
    public class SharingPolicyActivatorTester
    {
        private SharingGraph _graph;
        private List<ISharingPolicy> _policies;
        private SharingPolicyActivator _activator;
        private IPackageLog _packageLog;
        private SharingLogsCache _sharingLogs;

        [SetUp]
        public void beforeEach()
        {
            _policies = new List<ISharingPolicy> {MockRepository.GenerateMock<ISharingPolicy>()};
            _graph = new SharingGraph();
            _packageLog = MockRepository.GenerateMock<IPackageLog>();
            _sharingLogs = new SharingLogsCache();
            _activator = new SharingPolicyActivator(_graph, _sharingLogs, _policies);
        }

        [Test]
        public void apply_policies_logs_policy()
        {
            _activator.ApplyPolicies(_packageLog);
            _packageLog.AssertWasCalled(x => x.Trace("Applying policy [{0}].", _policies[0].ToString()));
        }

        [Test]
        public void apply_policies_sets_provenance()
        {
            _activator.ApplyPolicies(_packageLog);
            _activator.Diagnostics.CurrentProvenance.ShouldEqual(_policies[0].ToString());
        }

        [Test]
        public void apply_policies_uses_diagnostics()
        {
            _activator.ApplyPolicies(_packageLog);
            _policies[0].AssertWasCalled(x => x.Apply(_packageLog, _activator.Diagnostics));
        }

        [Test]
        public void register_as_global_logs_operation()
        {
            _activator.RegisterAppGlobal(_packageLog);
            _packageLog.AssertWasCalled(x => x.Trace("Registering application as global sharing."));
        }

        [Test]
        public void register_as_global_invokes_diagnostics()
        {
            var inner = MockRepository.GenerateMock<ISharingRegistration>();
            var fake = new SharingRegistrationDiagnostics(inner, _sharingLogs);

            _activator.Diagnostics = fake;
            _activator.RegisterAppGlobal(_packageLog);

            fake.CurrentProvenance.ShouldEqual(FubuSparkConstants.HostOrigin);
            inner.AssertWasCalled(x => x.Global(FubuSparkConstants.HostOrigin));
        }

        [Test]
        public void compile_dependencies_logs_all_provenances()
        {
            var packages = new List<IPackageInfo>
            {
                new PackageInfo("a"), 
                new PackageInfo("b")
            };

            _activator.CompileDependencies(packages, _packageLog);

            _packageLog.AssertWasCalled(x => x.Trace("Compiling dependencies for [{0}]", "a, b, Host"));
        }

        [Test]
        public void compile_dependencies_implicit_adds_host_as_provenance()
        {
            var packages = new List<IPackageInfo>
            {
                new PackageInfo("a"), 
                new PackageInfo("b")
            };

            _graph.Global("x");
            _activator.CompileDependencies(packages, _packageLog);

            _graph.SharingsFor(FubuSparkConstants.HostOrigin).ShouldContain("x");
        }
    }
}