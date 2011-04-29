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
        private readonly IDirectiveTypeRegistry _registry;
        private readonly IDeploymentDiagnostics _diagnostics;

        public DeploymentController(IProfileReader reader, IDirectiveTypeRegistry registry, IDeploymentDiagnostics diagnostics)
        {
            _reader = reader;
            _diagnostics = diagnostics;
            _registry = registry;
        }

        public void Deploy()
        {
            var hosts = _reader.Read();

            hosts.Each(h => h.BuildDirectives(_registry));

            throw new NotImplementedException();

            WriteToFile(_diagnostics);
        }

        private void WriteToFile(IDeploymentDiagnostics diagnostics)
        {
            var path = Path.GetFullPath("run.log");
            File.Delete(path);

            Console.WriteLine(path);
            //diagnostics.ForEach(log=>
            //{
            //    File.AppendAllText(path, log.Description);
            //    File.AppendAllText(path, log.FullTraceText());
            //    File.AppendAllText(path, System.Environment.NewLine);
            //});
        }
    }
}