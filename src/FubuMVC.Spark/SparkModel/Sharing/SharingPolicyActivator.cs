using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    // UT
    public class SharingPolicyActivator : IActivator
    {
        private readonly SharingGraph _graph;
        private readonly SharingLogsCache _logs;
        private readonly IEnumerable<ISharingPolicy> _policies;
        private readonly SharingRegistrationDiagnostics _diagnostics;

        public SharingPolicyActivator(SharingGraph graph, SharingLogsCache logs, IEnumerable<ISharingPolicy> policies)
        {
            _graph = graph;
            _logs = logs;
            _policies = policies;
            _diagnostics = new SharingRegistrationDiagnostics(_graph, _logs);
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _policies.Each(p =>
            {
                var policyName = p.ToString();
                
                log.Trace("Applying policy [{0}].", policyName);                
                _diagnostics.SetCurrentProvenance(policyName);                
                
                p.Apply(log, _diagnostics);
            });

            log.Trace("Registering application as global sharing.");

            _diagnostics.SetCurrentProvenance(FubuSparkConstants.HostOrigin);
            _diagnostics.Global(FubuSparkConstants.HostOrigin);
            
            var provenances = packages.Select(p => p.Name).Union(new[] { FubuSparkConstants.HostOrigin }).ToArray();
            log.Trace("Compiling dependencies for [{0}]", provenances.Join(", "));
            
            _graph.CompileDependencies(provenances);
        }

        public SharingRegistrationDiagnostics Diagnostics
        {
            get { return _diagnostics; }
        }
    }
}