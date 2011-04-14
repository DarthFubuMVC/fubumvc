using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Packaging;
using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Endpoints.Packaging
{
    public class LogEndpoint
    {
        public PackageDiagnosticsModel Get(PackageDiagnosticsRequestModel request)
        {
            // Might want to do some querying via json so let's flatten the logs - maybe map them?
            var logs = new List<PackageDiagnosticsLogModel>();
            
            // Nothing really gained here by mocking this so let's hit it directly
            PackageRegistry
                .Diagnostics
                .EachLog((target, log) => logs.Add(new PackageDiagnosticsLogModel
                                                       {
                                                           Type = PackagingDiagnostics.GetTypeName(target),
                                                           Description = target.ToString(),
                                                           Provenance = log.Provenance,
                                                           Timing = log.TimeInMilliseconds.ToString(),
                                                           FullTraceText = log.FullTraceText(),
                                                           Success = log.Success
                                                       }));

            return new PackageDiagnosticsModel
                       {
                           Logs = logs
                       };
        }
    }
}