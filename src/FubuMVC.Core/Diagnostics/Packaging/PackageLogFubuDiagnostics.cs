using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    public class PackageLogFubuDiagnostics
    {
        private readonly FubuRuntime _runtime;

        public PackageLogFubuDiagnostics(FubuRuntime runtime)
        {
            _runtime = runtime;
        }

        [System.ComponentModel.Description("Application Startup")]
        public PackageDiagnosticsModel get_package_logs(PackageDiagnosticsRequestModel request)
        {
            // Might want to do some querying via json so let's flatten the logs - maybe map them?
            var logs = new List<PackageDiagnosticsLogModel>();

            // Nothing really gained here by mocking this so let's hit it directly
            _runtime
                .ActivationDiagnostics
                .EachLog((target, log) => logs.Add(new PackageDiagnosticsLogModel{
                    Type = ActivationDiagnostics.GetTypeName(target),
                    Description = target.ToString(),
                    Provenance = log.Provenance,
                    FullTraceText = log.FullTraceText(),
                    Success = log.Success
                }));

            return new PackageDiagnosticsModel{
                logs = logs
            };
        }
    }
}