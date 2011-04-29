using System;
using System.Collections.Generic;
using System.IO;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Parsing;
using System.Linq;

namespace Bottles.Deployment.Runtime
{
    public class DeploymentController : IDeploymentController
    {
        private readonly IProfileReader _reader;
        private readonly IDeploymentDiagnostics _diagnostics;
        private readonly IDirectiveRunnerFactory _factory;

        public DeploymentController(IProfileReader reader, IDeploymentDiagnostics diagnostics, IDirectiveRunnerFactory factory)
        {
            _reader = reader;
            _diagnostics = diagnostics;
            _factory = factory;
        }

        public void Deploy()
        {
            // need to log inside of reader
            var hosts = _reader.Read();

            var runners = _factory.BuildRunners(hosts);

            runners.Each(x => x.InitializeDeployment());
            runners.Each(x => x.Deploy());
            runners.Each(x => x.FinalizeDeployment());

            // TODO -- write more to the console.
            // TODO -- 


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