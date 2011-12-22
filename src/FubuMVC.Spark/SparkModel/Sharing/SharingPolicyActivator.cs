using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    public class SharingPolicyActivator : IActivator
    {
        private readonly SharingGraph _graph;
        private readonly SharingLogsCache _logs;
        private readonly IEnumerable<ISharingPolicy> _policies;

        public SharingPolicyActivator(SharingGraph graph, SharingLogsCache logs, IEnumerable<ISharingPolicy> policies)
        {
            _graph = graph;
            _logs = logs;
            _policies = policies;

            Diagnostics = new SharingRegistrationDiagnostics(_graph, _logs);
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            ApplyPolicies(log);
            RegisterAppGlobal(log);            
            CompileDependencies(packages, log);
        }

        public void ApplyPolicies(IPackageLog log)
        {
            _policies.Each(p =>
            {
                var policyName = p.ToString();

                log.Trace("Applying policy [{0}].", policyName);
                Diagnostics.SetCurrentProvenance(policyName);

                p.Apply(log, Diagnostics);
            });            
        }

        public void CompileDependencies(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var provenances = packages.Select(p => p.Name).Union(new[] { FubuSparkConstants.HostOrigin }).ToArray();
            
            log.Trace("Compiling dependencies for [{0}]", provenances.Join(", "));
            
            _graph.CompileDependencies(provenances);            
        }

        // I would rather have this as a ISharingPolicy, but don't know how to ensure it is applied last.
        public void RegisterAppGlobal(IPackageLog log)
        {
            log.Trace("Registering application as global sharing.");
            
            Diagnostics.SetCurrentProvenance(FubuSparkConstants.HostOrigin);
            Diagnostics.Global(FubuSparkConstants.HostOrigin);            
        }

        public SharingRegistrationDiagnostics Diagnostics { get; set; }
    }
}