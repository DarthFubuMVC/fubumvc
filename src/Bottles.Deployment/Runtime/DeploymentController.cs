using System;
using System.Collections.Generic;
using System.IO;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Parsing;

namespace Bottles.Deployment.Runtime
{
    public class DeploymentController : IDeploymentController
    {
        private readonly IProfileReader _reader;
        private readonly IDirectiveRunner _runner;
        private readonly IDirectiveTypeRegistry _registry;
        private readonly IDeploymentDiagnostics _diagnostics;

        public DeploymentController(IProfileReader reader, IDirectiveRunner runner, IDirectiveTypeRegistry registry, IDeploymentDiagnostics diagnostics)
        {
            _reader = reader;
            _diagnostics = diagnostics;
            _registry = registry;
            _runner = runner;
        }

        public void Deploy()
        {
            var hosts = _reader.Read();

            hosts.Each(h => h.BuildDirectives(_registry));

            _runner.Deploy(hosts);

            WriteToFile(_diagnostics);
        }

        private void WriteToFile(IDeploymentDiagnostics diagnostics)
        {
            var path = Path.GetFullPath("run.log");

            Console.WriteLine(path);
            diagnostics.ForEach(log=>
            {
                File.AppendAllText(path, log.FullTraceText());
            });
        }
    }
}