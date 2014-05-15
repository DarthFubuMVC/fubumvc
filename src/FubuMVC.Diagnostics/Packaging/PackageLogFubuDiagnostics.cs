using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Diagnostics.Packaging
{
    public class PackageLogFubuDiagnostics
    {
        [System.ComponentModel.Description("Application Startup")]
        public PackageDiagnosticsModel get_package_logs(PackageDiagnosticsRequestModel request)
        {
            // Might want to do some querying via json so let's flatten the logs - maybe map them?
            var logs = new List<PackageDiagnosticsLogModel>();

            // Nothing really gained here by mocking this so let's hit it directly
            PackageRegistry
                .Diagnostics
                .EachLog((target, log) => logs.Add(new PackageDiagnosticsLogModel{
                    Type = BottlingDiagnostics.GetTypeName(target),
                    Description = target.ToString(),
                    Provenance = log.Provenance,
                    FullTraceText = log.FullTraceText(),
                    Success = log.Success
                }));

            return new PackageDiagnosticsModel{
                Logs = logs
            };
        }
    }
}